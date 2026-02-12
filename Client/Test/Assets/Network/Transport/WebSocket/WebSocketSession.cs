using Network.API;
using Network.Core.Frame;
using Network.Core.Tick;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Transport.WebSocket
{
    internal class WebSocketSession : ConnectionSession<ClientWebSocket>
    {
        public WebSocketSession(string host, int port) : base(host, port)
        {
            this.host = host;
            this.port = port;
        }

        protected override async Task OnConnectAsync(string token)
        {
            channel = new ClientWebSocket();
            cts = new CancellationTokenSource();

            string url = $"ws://{host}:{port}/ws?token={Uri.EscapeDataString(token)}";
            await channel.ConnectAsync(new Uri(url), CancellationToken.None);
            StartHeartbeatLoop();
            Debug.Log("已连接服务器");
        }

        protected override async void OnDisconnectAsync()
        {
            if (channel != null && channel.State == WebSocketState.Open)
            {
                Debug.Log("断开websocket连接");
                StopHeartbeatLoop();
                await SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect");
            }
            cts?.Cancel();
            channel?.Dispose();
        }
        
        public override void OnMessageReceived(string msg)
        {
            WebSocketResult<object> wsResult = JsonConvert.DeserializeObject<WebSocketResult<object>>(msg);
            if (wsResult == null) return;

            if (wsResult.Type == EWsMessageType.Normal)
            {
                string pattern = wsResult.Pattern;
                ApiManager.HandleMessage(pattern, msg);
            }
            else if (wsResult.Type == EWsMessageType.Heartbeat)
            {
                // 处理心跳响应（如果需要）
                FrameManager.Instance.RefreshServerFrame(wsResult.ServerFrame, wsResult.Timestamp);
            }
            else if (wsResult.Type == EWsMessageType.Relogin)
            {
                // 处理重新登录逻辑
                Debug.Log("收到重新登录请求，需重新认证身份");
                NetworkManager.Instance.HttpLogin();
            }
            else if (wsResult.Type == EWsMessageType.FrameSync)
            {
                // 处理帧同步消息
                FrameManager.Instance.RefreshServerFrame(wsResult.ServerFrame, wsResult.Timestamp);
                string pattern = wsResult.Pattern;
                ApiManager.HandleMessage(pattern, msg);
            }
        }
        protected override async void OnSendMessageAsync(string wsMessage)
        {
            if (channel == null || channel.State != WebSocketState.Open)
            {
                Debug.LogWarning("WebSocket 未连接，无法发送消息");
                return;
            }
            var buffer = Encoding.UTF8.GetBytes(wsMessage);
            var segment = new ArraySegment<byte>(buffer);
            await channel.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
        protected override async void ReceiveLoopAsync()
        {
            var buffer = new byte[1024];

            while (!cts.Token.IsCancellationRequested && channel.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                try { result = await channel.ReceiveAsync(buffer, cts.Token); }
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
                if (channel != null &&
                    (channel.State == WebSocketState.Open ||
                     channel.State == WebSocketState.CloseReceived))
                {
                    await channel.CloseAsync(status, reason, CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Debug.Log($"WebSocket close exception (ignored): {e.Message}");
            }
            finally
            {
                cts?.Cancel();
                channel?.Dispose();
            }
        }
        #region heartbeat
        private void StartHeartbeatLoop()
        {
            if (heartbeatRunning) return;// 避免重入
            heartbeatRunning = true;

            tickHandle = TickManager.Instance.RegisterTick(heartbeatIntervalMs, OnHeartbeatTick);
        }
        protected async void OnHeartbeatTick()
        {
            WebSocketMessage<string> wsMessage = new()
            {
                Type = EWsMessageType.Heartbeat,
                Pattern = "/ping",
                Data = "ping"
            };
            string jsonMessage = JsonConvert.SerializeObject(wsMessage);
            var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
            var segment = new ArraySegment<byte>(messageBytes);
            if (channel.State == WebSocketState.Open)
            {
                try { await channel.SendAsync(segment, WebSocketMessageType.Text, true, cts.Token); }
                catch { Debug.LogError("向服务器发送ping消息失败！"); }
            }
        }
        private void StopHeartbeatLoop()
        {
            heartbeatRunning = false;
            tickHandle?.Stop();
        }
        #endregion
    }
}
