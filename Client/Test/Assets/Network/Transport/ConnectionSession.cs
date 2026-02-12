using Network.API;
using Network.Core.Frame;
using Network.Core.Tick;
using Network.Transport.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

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
        public void SendMessageAsync(string msg)
        {
            OnSendMessageAsync(msg);
        }
        protected abstract Task OnConnectAsync(string token);
        protected abstract void OnDisconnectAsync();
        public abstract void OnMessageReceived(string msg);
        protected abstract void OnSendMessageAsync(string msg);
        protected abstract void ReceiveLoopAsync();
    }
}
