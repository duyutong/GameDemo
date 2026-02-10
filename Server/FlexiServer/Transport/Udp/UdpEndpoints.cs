using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlexiServer.Transport.Udp
{
    public static class UdpEndpoints
    {
        private static CancellationTokenSource? cts;
        private static UdpClient ? udpClient;
        private static UdpTransport? transport;
        public static async Task StartUdpListen(this WebApplication app,int port)
        {
            udpClient = new UdpClient(port);
            cts = new CancellationTokenSource();

            transport = app.Services.GetService<UdpTransport>();
            if (transport == null) { udpClient.Dispose(); return; }
            transport.SetClient(udpClient, cts);

            await MessageReceiveLoop();
        }
        private async static Task MessageReceiveLoop() 
        {
            while (cts != null && !cts.Token.IsCancellationRequested && udpClient != null)
            {
                // 异步接收数据
                UdpReceiveResult result = await udpClient.ReceiveAsync();

                // 处理接收到的数据
                transport?.ReceiveFromRemote(result.Buffer, result.RemoteEndPoint);
            }
        }


    }
}
