using Network.Core.Tick;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Network.Transport
{
    public abstract class ConnectionSession
    {
        public string host;
        public int port;
        public ConnectionSession(string host, int port)
        {
            this.host = host;
            this.port = port;
        }
    }    
    public abstract class ConnectionSession<T> : ConnectionSession where T :IDisposable
    {
        public T channel;
        public CancellationTokenSource cts;
        public TickHandle tickHandle;
        public int heartbeatIntervalMs = 5000;
        public bool heartbeatRunning = false;

        protected ConnectionSession(string host, int port) : base(host, port)
        {
            this.host = host;
            this.port = port;
        }

        public async Task ConnectAsync(string token)
        {
            await OnConnectAsync(token);
            ReceiveLoopAsync();
        }
        public void DisconnectAsync()
        {
            OnDisconnectAsync();
        }
        public void SendMessageAsync<TData>(string pattern, string path, TData messageObj)
        {
            OnSendMessageAsync(pattern, path, messageObj);
        }
        protected abstract Task OnConnectAsync(string token);
        protected abstract void OnDisconnectAsync();
        public abstract void OnMessageReceived(byte[] buffer);
        protected abstract void OnSendMessageAsync<TData>(string pattern, string path, TData messageObj);
        protected abstract void ReceiveLoopAsync();
    }
}
