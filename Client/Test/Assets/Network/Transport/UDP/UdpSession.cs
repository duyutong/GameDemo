using Network.API;
using Network.Core.Frame;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Network.Transport.Udp
{
    internal class UdpSession : ConnectionSession<UdpClient>
    {
        private FrameManager frameManager => FrameManager.Instance;
        public UdpSession(string host, int port) : base(host, port) { }

        public override void OnMessageReceived(string msg)
        {
            UdpResult<object> udpResult = JsonConvert.DeserializeObject<UdpResult<object>>(msg);
            if (udpResult == null) return;
            Debug.Log($"UDP  ’µΩ: {udpResult.Data}");

            frameManager.RefreshServerFrame(udpResult.ServerFrame, udpResult.Timestamp);
            string pattern = udpResult.Pattern;
            ApiManager.HandleUdpMessage(pattern, msg);
        }
        public async Task Connect() { await ConnectAsync(string.Empty); }
        protected override Task OnConnectAsync(string token)
        {
            channel = new UdpClient();
            cts = new CancellationTokenSource();

            channel?.Connect(host, port);
            return Task.CompletedTask;
        }

        protected override void OnDisconnectAsync()
        {
            cts?.Cancel();
            channel?.Close();
            channel?.Dispose();
        }

        protected override async void OnSendMessageAsync(string udpMessage)
        {
            byte[] data = Encoding.UTF8.GetBytes(udpMessage);
            await channel.SendAsync(data, data.Length);
        }

        protected override async void ReceiveLoopAsync()
        {
            while (cts != null && !cts.Token.IsCancellationRequested && channel != null)
            {
                try
                {
                    UdpReceiveResult result = await channel.ReceiveAsync();
                    string msg = Encoding.UTF8.GetString(result.Buffer);
                    OnMessageReceived(msg);
                }
                catch (Exception ex) { }
                
            }
        }
    }
}
