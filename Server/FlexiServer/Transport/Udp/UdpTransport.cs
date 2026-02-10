using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Transport.Interface;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlexiServer.Transport.Udp
{
    public class UdpTransport(TickManager tickManager, FrameManager frameManager) : ITransport
    {
        private readonly TickManager tickManager = tickManager;
        private readonly FrameManager frameManager = frameManager;
        private UdpClient udpClient;
        private CancellationTokenSource cts;
        private ConcurrentDictionary<string, ClientConnection> udpClients = new();  //Account to IPEndPoint
        private Action<SClientConnectData, string, string>? OnReceived;
        public void SetClient(UdpClient _clien, CancellationTokenSource _cts) 
        {
            udpClient = _clien;
            cts = _cts;
        }
        public void SetConnectionStateChangedListener(Action<SClientConnectData, EPlayerConnectionState> onConnectionStateChanged)
        {

        }
        private void OnMessageReceived(SClientConnectData connectData, string msg)
        {
            UdpMessage<object>? udpMessage = JsonConvert.DeserializeObject<UdpMessage<object>>(msg);
            if (udpMessage == null) return;

            string pattern = udpMessage.Pattern;
            OnReceived?.Invoke(connectData, pattern, msg);
        }
        public void SetMessageReceivedListener(Action<SClientConnectData, string, string> receivedCall)
        {
            OnReceived = receivedCall;
        }

        public async void SendMessage(string account, string message)
        {
            if (!udpClients.ContainsKey(account)) return;
            if (!udpClients.TryGetValue(account, out var client)) return;
            if (client.ClientEndPoint == null) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(data, client.ClientEndPoint);
        }
        public void Start() { }

        public void Stop()
        {
            OnReceived = null;
            udpClients.Clear();
            cts?.Cancel();
        }
        public void ReceiveFromRemote(byte[] buffer, IPEndPoint clientRemote)
        {
            // 解析客户端发送的数据
            string msg = Encoding.UTF8.GetString(buffer);
            if (string.IsNullOrEmpty(msg)) return;

            UdpMessage<object>? udpMessage = JsonConvert.DeserializeObject<UdpMessage<object>>(msg);
            if (udpMessage == null) return;

            string account = udpMessage.Account;
            if (!udpClients.ContainsKey(account))
            {
                ClientConnection client = new ClientConnection();
                client.Account = account;
                client.ClientEndPoint = clientRemote;

                udpClients.TryAdd(account, client);
            }
            else 
            {
                udpClients[account].ClientEndPoint = clientRemote;
            }

            SClientConnectData clientConnect = new SClientConnectData();
            clientConnect.Account = account;
            OnMessageReceived(clientConnect, msg);
        }
        private class ClientConnection {
            public string Account { get; set; } = "";
            public IPEndPoint? ClientEndPoint { get; set; }
        }
    }
}
