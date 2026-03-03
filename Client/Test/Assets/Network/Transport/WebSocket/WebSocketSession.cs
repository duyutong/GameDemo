using Assets.Network.API.ErrorCode;
using Network.API;
using Network.Core.Frame;
using Network.Core.Tick;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
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

        public override void OnMessageReceived(byte[] buffer)
        {
            WebSocketResult<object> wsResult = DeserializeWsResult<object>(buffer);
            if (wsResult == null) return;

            if (wsResult.Code == 200)
            {
                FrameManager.Instance.RefreshServerFrame(wsResult.ServerFrame, wsResult.Timestamp);
                string pattern = wsResult.Pattern;
                ApiManager.HandleWebsocketMessage(pattern, wsResult);
            }
            else if (wsResult.Code == (int)ErrorCode.TokenExpired)
            {
                Debug.Log("Token过期，正在重新登录...");
                NetworkManager.Instance.HttpLogin();
                return;
            }
        }
        protected override async void OnSendMessageAsync<TData>(string pattern, string path, TData messageObj)
        {
            if (channel == null || channel.State != WebSocketState.Open)
            {
                Debug.LogWarning("WebSocket 未连接，无法发送消息");
                return;
            }

            WebSocketMessage<TData> wsMessage = new();
            wsMessage.InputFrame = FrameManager.Instance.LocalCurrentFrame;
            wsMessage.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            wsMessage.Pattern = pattern;
            wsMessage.Path = path;
            wsMessage.Data = messageObj;

            var buffer = SerializeWsMessage(wsMessage);
            var lenBytes = BitConverter.GetBytes(buffer.Length); // 4字节长度前缀
            var sendBytes = lenBytes.Concat(buffer).ToArray();
            var segment = new ArraySegment<byte>(sendBytes);

            await channel.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        private byte[] SerializeWsMessage<T>(WebSocketMessage<T> message)
        {
            if (GlobalSetting.Instance.format == ETransportFormat.Json)
            {
                string json = JsonConvert.SerializeObject(message);
                return Encoding.UTF8.GetBytes(json);
            }
            else
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }
        private WebSocketResult<TData> DeserializeWsResult<TData>(byte[] bytes)
        {
            if (GlobalSetting.Instance.format == ETransportFormat.Json)
            {
                if (bytes.Length < 4) return null!;
                int jsonLength = BitConverter.ToInt32(bytes, 0);
                if (bytes.Length < 4 + jsonLength) return null!;

                byte[] jsonBytes = new byte[jsonLength];
                Array.Copy(bytes, 4, jsonBytes, 0, jsonLength);

                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonConvert.DeserializeObject<WebSocketResult<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<WebSocketResult<TData>>(ms);
            }
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

                OnMessageReceived(buffer);
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
        protected void OnHeartbeatTick()
        {
            try { SendMessageAsync("/ping", "", "ping"); }
            catch { Debug.LogError("发送ping消息失败！"); }
        }
        private void StopHeartbeatLoop()
        {
            heartbeatRunning = false;
            tickHandle?.Stop();
        }
        #endregion
    }
}
