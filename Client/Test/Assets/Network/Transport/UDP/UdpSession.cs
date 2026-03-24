using Assets.Network.Transport;
using Network.API;
using Network.Core.Frame;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Network.Transport.Udp
{
    internal class UdpSession : ConnectionSession<UdpClient>
    {
        private FrameManager frameManager => FrameManager.Instance;
        public UdpSession(string host, int port) : base(host, port) { }

        public override void OnMessageReceived(byte[] buffer)
        {
            Debug.Log($"Received UDP message, length: {buffer.Length}");
            var udpResult = buffer.ConvertData<UdpResult>();
            if (udpResult == null) return;

            frameManager.RefreshServerFrame(udpResult.ServerFrame, udpResult.Timestamp);
            string pattern = udpResult.Pattern;
            ApiManager.HandleUdpMessage(pattern, udpResult);
        }
        /// <summary>
        /// 伪连接，事实上只初始化了一些变量
        /// </summary>
        /// <returns></returns>
        public async Task Connect() { await ConnectAsync(string.Empty); }
        protected override Task OnConnectAsync(string token)
        {
            channel = new UdpClient();
            cts = new CancellationTokenSource();

            // 客户端不连接，否则重连后因为端口变化，会导致系统的连接检测失败
            //channel?.Connect(host, port);
            return Task.CompletedTask;
        }

        protected override void OnDisconnectAsync()
        {
            cts?.Cancel();
            channel?.Close();
            channel?.Dispose();
        }

        protected override async void OnSendMessageAsync<TData>(string pattern, string path, TData messageObj)
        {
            if (channel == null) return;

            UdpMessage udpMessage = new UdpMessage();
            udpMessage.Account = NetworkManager.Instance.Account;
            udpMessage.InputFrame = FrameManager.Instance.LocalCurrentFrame;
            udpMessage.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            udpMessage.Pattern = pattern;
            udpMessage.Path = path;
            udpMessage.Data = messageObj.ToBytes();

            var buffer = udpMessage.ToBytes();
            await channel.SendAsync(buffer, buffer.Length, host, port);
        }

        protected override async void ReceiveLoopAsync()
        {
            while (cts != null && !cts.Token.IsCancellationRequested && channel != null)
            {
                try
                {
                    UdpReceiveResult result = await channel.ReceiveAsync();
                    OnMessageReceived(result.Buffer);
                }
                catch (Exception) { }

            }
        }
    }
}
