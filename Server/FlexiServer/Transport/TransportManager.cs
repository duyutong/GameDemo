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
        private event Action<string, string, string, byte[]>? ClientMsgEvent;
        public void AddClientConnHandler(Action<string, string, EPlayerConnectionState> handler)
            => ClientConnEvent += handler;
        public void RemoveClientConnHandler(Action<string, string, EPlayerConnectionState> handler)
            => ClientConnEvent -= handler;
        public void AddClientMsgHandler(Action<string, string, string, byte[]> handler)
            => ClientMsgEvent += handler;
        public void RemoveClientMsgHandler(Action<string, string, string, byte[]> handler)
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
        private void OnMessageReceived(SClientConnectData connectData, string pattern, byte[] buffer)
        {
            string account = connectData.Account;
            string clientId = connectData.ClientId;
            ClientMsgEvent?.Invoke(pattern, clientId, account, buffer);
        }
        private static TTransport GetTransport<TTransport>() where TTransport : class, ITransport 
        {
            foreach (var tra in transports)
            {
                var transport = tra as TTransport;
                if (transport != null) return transport;
            }
            return null!;
        }
        public static void SendMessageToClient<TTransport, TData>(List<string> clientKeys, string pattern, string path, TData? data) where TTransport : class, ITransport
        {
            TTransport transport = GetTransport<TTransport>();
            if (transport == null) return;
            
            foreach (var clientKey in clientKeys)
                transport.SendMessage(clientKey, pattern, path, data);
        }
        public static void SendMessageToClient<TTransport, TData>(string clientKey, string pattern, string path, TData? data) where TTransport : class, ITransport
        {
            TTransport transport = GetTransport<TTransport>();
            if (transport == null) return;

            transport.SendMessage(clientKey, pattern, path, data);
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
