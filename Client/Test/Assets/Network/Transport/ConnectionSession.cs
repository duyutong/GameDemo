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
    public abstract class ConnectionSession<T> : ConnectionSession where T : System.Net.WebSockets.WebSocket
    {
        public T ws;
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
            StartHeartbeatLoop();
        }
        public void DisconnectAsync()
        {
            OnDisconnectAsync();
            StopHeartbeatLoop();
        }
        public void SendMessageAsync(string wsMessage)
        {
            OnSendMessageAsync(wsMessage);
        }
        protected abstract Task OnConnectAsync(string token);
        protected abstract void OnDisconnectAsync();
        protected abstract void OnHeartbeatTick();
        public abstract void OnMessageReceived(string msg);
        protected abstract void OnSendMessageAsync(string wsMessage);

        private void StartHeartbeatLoop()
        {
            heartbeatRunning = false;
            tickHandle = TickManager.Instance.RegisterTick(heartbeatIntervalMs, HeartbeatLoop);
        }
        private void HeartbeatLoop()
        {
            if (heartbeatRunning) return;// 避免重入
            OnHeartbeatTick();
        }
        private void StopHeartbeatLoop()
        {
            heartbeatRunning = false;
            tickHandle?.Stop();
        }
        private async void ReceiveLoopAsync()
        {
            var buffer = new byte[1024];

            while (!cts.Token.IsCancellationRequested && ws.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                try { result = await ws.ReceiveAsync(buffer, cts.Token); }
                catch { break; }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.Log("服务器要求断开连接");
                    await SafeCloseAsync((WebSocketCloseStatus)result.CloseStatus, result.CloseStatusDescription);

                    break;
                }
                string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                OnMessageReceived(msg);
            }
        }
        private int _disconnectFlag = 0;

        protected async Task SafeCloseAsync(WebSocketCloseStatus status, string reason)
        {
            // 确保只执行一次
            if (Interlocked.Exchange(ref _disconnectFlag, 1) == 1) return;

            try
            {
                if (ws != null &&
                    (ws.State == WebSocketState.Open ||
                     ws.State == WebSocketState.CloseReceived))
                {
                    await ws.CloseAsync(status, reason, CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"WebSocket close exception (ignored): {e.Message}");
            }
            finally
            {
                cts?.Cancel();
                ws?.Dispose();
            }
        }

    }
}
