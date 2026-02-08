using Network.API;
using Network.Core.Frame;
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
            ws = new ClientWebSocket();
            cts = new CancellationTokenSource();

            string url = $"ws://{host}:{port}/ws?token={Uri.EscapeDataString(token)}";
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            Debug.Log("已连接服务器");
        }

        protected override async void OnDisconnectAsync()
        {
            if (ws != null && ws.State == WebSocketState.Open)
            {
                Debug.Log("断开websocket连接");
                await SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect");
            }
            cts?.Cancel();
            ws?.Dispose();
        }
        protected override async void OnHeartbeatTick()
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
            if (ws.State == WebSocketState.Open)
            {
                try { await ws.SendAsync(segment, WebSocketMessageType.Text, true, cts.Token); }
                catch { Debug.LogError("向服务器发送ping消息失败！"); }
            }
        }
        public override void OnMessageReceived(string msg)
        {
            WebSocketResult<object> wsMessage = JsonConvert.DeserializeObject<WebSocketResult<object>>(msg);
            if (wsMessage == null) return;

            if (wsMessage.Type == EWsMessageType.Normal)
            {
                string pattern = wsMessage.Pattern;
                ApiManager.HandleMessage(pattern, msg);
            }
            else if (wsMessage.Type == EWsMessageType.Heartbeat)
            {
                // 处理心跳响应（如果需要）
            }
            else if (wsMessage.Type == EWsMessageType.Relogin)
            {
                Debug.Log("收到重新登录请求，需重新认证身份");
                // 处理重新登录逻辑
                NetworkManager.Instance.HttpLogin();
            }
            else if (wsMessage.Type == EWsMessageType.FrameSync)
            {
                // 处理帧同步消息
                FrameManager.Instance.RefreshServerFrame(wsMessage.ServerFrame, wsMessage.Timestamp);
            }
        }
        protected override async void OnSendMessageAsync(string wsMessage)
        {
            if (ws == null || ws.State != WebSocketState.Open)
            {
                Debug.LogWarning("WebSocket 未连接，无法发送消息");
                return;
            }
            var buffer = Encoding.UTF8.GetBytes(wsMessage);
            var segment = new ArraySegment<byte>(buffer);
            await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
