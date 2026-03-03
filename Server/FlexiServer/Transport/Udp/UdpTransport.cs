using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Transport.Interface;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlexiServer.Transport.Udp
{
    public class UdpTransport(FrameManager frameManager) : ITransport
    {
        private readonly FrameManager frameManager = frameManager;

        private UdpClient? udpClient;
        private CancellationTokenSource?cts;
        private ConcurrentDictionary<string, ClientConnection> udpClients = new();  //Account to IPEndPoint
        private Action<SClientConnectData, string, byte[]>? OnReceived;
        public void SetClient(UdpClient _clien, CancellationTokenSource _cts) 
        {
            udpClient = _clien;
            cts = _cts;
        }
        public void SetConnectionStateChangedListener(Action<SClientConnectData, EPlayerConnectionState> onConnectionStateChanged)
        {

        }
        private void OnMessageReceived(SClientConnectData connectData, byte[] buffer)
        {
            UdpMessageHeader udpMessage = TransportUtil.DeserializeUdpMessageHeader(buffer);
            if (udpMessage == null) return;

            string pattern = udpMessage.Pattern;
            OnReceived?.Invoke(connectData, pattern, buffer);
        }
        public void SetMessageReceivedListener(Action<SClientConnectData, string, byte[]> receivedCall)
        {
            OnReceived = receivedCall;
        }

        public async void SendMessage<TData>(string clientKey, string pattern, string path, TData data)
        {
            if (!udpClients.ContainsKey(clientKey)) return;
            if (!udpClients.TryGetValue(clientKey, out var client)) return;
            if (client.ClientEndPoint == null) return;
            if (udpClient == null) return;

            UdpResult<TData> sendMsg = new();
            sendMsg.Pattern = pattern;
            sendMsg.Path = path;
            sendMsg.Data = data;
            sendMsg.ServerFrame = frameManager.ServerCurrentFrame;
            sendMsg.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var datagram = TransportUtil.SerializeUdpResult(sendMsg);
            await udpClient.SendAsync(datagram, client.ClientEndPoint);
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

            UdpMessageHeader udpMessage = TransportUtil.DeserializeUdpMessageHeader(buffer);
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
            OnMessageReceived(clientConnect, buffer);
        }
        private class ClientConnection {
            public string Account { get; set; } = "";
            public IPEndPoint? ClientEndPoint { get; set; }
        }
    }
}
