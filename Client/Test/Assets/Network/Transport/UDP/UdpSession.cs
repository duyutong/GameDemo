using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Network.Transport.Udp
{
    internal class UdpSession : ConnectionSession<UdpClient>
    {
        public UdpSession(string host, int port) : base(host, port) { }

        public override void OnMessageReceived(string msg)
        {
            Console.WriteLine($"UDP 收到: {msg}");
        }
        public void Connect() { OnConnectAsync(string.Empty); }
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
            while (!cts.Token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await channel.ReceiveAsync();
                    string msg = Encoding.UTF8.GetString(result.Buffer);
                    OnMessageReceived(msg);
                }
                catch (OperationCanceledException)
                {
                    break; // 取消接收循环
                }
            }
        }
    }
}
