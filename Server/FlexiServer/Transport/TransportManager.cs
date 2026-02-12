using FlexiServer.Core;
using FlexiServer.Transport.Interface;
using System.Security.Claims;

namespace FlexiServer.Transport
{
    public class TransportManager
    {
        private readonly static List<ITransport> transports = new List<ITransport>();

        #region 事件注册/注销
        private event Action<string, string, EPlayerConnectionState>? ClientConnEvent;
        private event Action<string, string, string, string>? ClientMsgEvent;
        public void AddClientConnHandler(Action<string, string, EPlayerConnectionState> handler)
            => ClientConnEvent += handler;
        public void RemoveClientConnHandler(Action<string, string, EPlayerConnectionState> handler)
            => ClientConnEvent -= handler;
        public void AddClientMsgHandler(Action<string, string, string, string> handler)
            => ClientMsgEvent += handler;
        public void RemoveClientMsgHandler(Action<string, string, string, string> handler)
            => ClientMsgEvent -= handler;
        #endregion
        public void RgiestTransport<T>(T? transport) where T : ITransport
        {
            if (transport == null) return;
            transport.SetMessageReceivedListener(OnMessageReceived);
            transport.SetConnectionStateChangedListener(OnConnectionStateChanged);
            transports.Add(transport);
        }
        private void OnConnectionStateChanged(SClientConnectData connectData, EPlayerConnectionState connectionState)
        {
            string account = connectData.Account;
            string clientId = connectData.ClientId;
            ClientConnEvent?.Invoke(clientId, account, connectionState);
        }
        private void OnMessageReceived(SClientConnectData connectData, string pattern, string msg)
        {
            string account = connectData.Account;
            string clientId = connectData.ClientId;
            ClientMsgEvent?.Invoke(pattern, clientId, account, msg);
        }
        public static void SendMessageToClient<T>(List<string> clienyKeys, string message) where T : class, ITransport
        {
            foreach (string key in clienyKeys)
                SendMessageToClient<T>(key, message);
        }
        public static void SendMessageToClient<T>(string clientKey, string message) where T : class, ITransport
        {
            foreach (var tra in transports)
            {
                var transport = tra as T;
                transport?.SendMessage(clientKey, message);
            }
        }
        public void OnApplicationStarted()
        {
            foreach (var transport in transports) transport.Start();
        }
        public void OnApplicationStopped()
        {
            foreach (var transport in transports) transport.Stop();
        }
    }
}
