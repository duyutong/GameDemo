using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Transport.Interface;
using Newtonsoft.Json;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace FlexiServer.Transport.Web
{
    public class WebSocketTransport(TickManager tickManager, FrameManager frameManager, TokenManager tokenService) : ITransport
    {
        private readonly TickManager tickManager = tickManager;
        private readonly FrameManager frameManager = frameManager;
        private readonly TokenManager tokenService = tokenService;

        // 保存所有连接
        private readonly ConcurrentDictionary<string, ClientConnection> clients = new();
        // 心跳间隔:5s
        private readonly int heartbeatIntervalMs = 5000;
        // 是否正在运行心跳
        private bool heartbeatRunning = false;

        private CancellationTokenSource? cts;

        private TickHandle? tickHandle;
        private Action<SClientConnectData, string, string>? OnReceived;
        private Action<SClientConnectData, EPlayerConnectionState>? OnConnStateChanged;
        public void Start()
        {
            cts = new CancellationTokenSource();
            StartHeartbeatLoop();
        }

        public void Stop()
        {
            cts?.Cancel();
            foreach (var client in clients.Values)
            {
                client.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Server stopping").Wait();
            }
            StopHeartbeatLoop();
        }
        public void SetConnectionStateChangedListener(Action<SClientConnectData, EPlayerConnectionState> onConnectionStateChanged)
        {
            OnConnStateChanged = onConnectionStateChanged;
        }
        public async Task RequestReLoginAsync(WebSocket ws)
        {
            if (ws.State != WebSocketState.Open) return;

            WebSocketResult<string> wsMessage = new WebSocketResult<string>();
            wsMessage.Type = EWsMessageType.Relogin;
            wsMessage.Data = "Token expired, please login again.";

            string msg = JsonConvert.SerializeObject(wsMessage);
            var buffer = Encoding.UTF8.GetBytes(msg);
            try
            {
                int timeoutMs = 2000;
                using var _cts = new CancellationTokenSource();
                _cts.CancelAfter(timeoutMs);
                await ws.SendAsync(buffer, WebSocketMessageType.Text, true, _cts.Token);
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Token expired", _cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RequestReLoginAsync failed: {ex.Message}");
            }
        }
        // 注册新客户端
        public async Task AddClient(string token, WebSocket ws)
        {
            string? account = tokenService.GetAccount(token);
            if (string.IsNullOrEmpty(account)) return;

            ClientConnection oldClient;
            ClientConnection client = new() { WebSocket = ws, LastActiveTime = DateTime.UtcNow };
            client.WebSocket = ws;
            client.Account = account;
            client.Token = token;
            client.LastActiveTime = DateTime.UtcNow;
            client.ClientId = GetOrGenerateClientId(account, out oldClient);

            // 挤掉线or重连？
            oldClient?.Cts.Cancel();
            oldClient?.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting");

            clients[client.ClientId] = client;
            await StartReceiveLoop(client.ClientId, client);
        }
        // 注销客户端
        public void RemoveClient(string clientId)
        {
            if (clients.TryRemove(clientId, out var client))
            {
                client.Cts.Cancel();
                client.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Removing client").Wait();
            }
        }
        private string GetOrGenerateClientId(string account, out ClientConnection oldClient)
        {
            ClientConnection client = clients.FirstOrDefault(kvp => kvp.Value.Account == account).Value;
            oldClient = client;
            string clientId = client != null ? client.ClientId : Guid.NewGuid().ToString();
            return clientId;
        }
        #region 信息收/发
        private async Task StartReceiveLoop(string clientId, ClientConnection client)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4096);
            var ws = client.WebSocket;
            SClientConnectData clientConnect = new SClientConnectData();

            try
            {
                while (ws != null && ws.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult request;

                    try
                    {
                        request = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), client.Cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // 收到取消请求，安全退出循环
                        break;
                    }
                    catch (WebSocketException)
                    {
                        // 发生 WebSocket 异常（例如远端强制关闭）
                        // 不再重复 CloseAsync，如果已经是 Aborted，则直接退出
                        if (ws.State != WebSocketState.Aborted)
                            await client.SafeCloseAsync(WebSocketCloseStatus.InternalServerError, "Receive exception");

                        Console.WriteLine("发生 WebSocket 异常（例如远端强制关闭） WebSocketManager");
                        break;
                    }

                    if (request == null) break;

                    if (request.MessageType == WebSocketMessageType.Close)
                    {
                        client.Cts.Cancel();
                        await client.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed connection");
                        ClientCloseConnect(clientId);
                        Console.WriteLine("客户端断开 WebSocketManager");
                        break;
                    }
                    else
                    {
                        // 客户端发任何消息时刷新上次活动时间
                        client.LastActiveTime = DateTime.UtcNow;

                        // 解析客户端发送的数据
                        var msg = Encoding.UTF8.GetString(buffer, 0, request.Count);
                        clientConnect.ClientId = clientId;
                        clientConnect.Account = client.Account;
                        OnMessageReceived(clientConnect, msg);
                    }
                }
            }
            finally
            {
                // 释放缓冲区和 WebSocket
                ArrayPool<byte>.Shared.Return(buffer);

                if (ws != null && ws.State != WebSocketState.Aborted)
                    await client.SafeCloseAsync(WebSocketCloseStatus.NormalClosure, "Loop ended");

                ws?.Dispose();
            }
        }

        private void OnMessageReceived(SClientConnectData connectData, string msg)
        {
            WebSocketMessage<object>? wsMessage = JsonConvert.DeserializeObject<WebSocketMessage<object>>(msg);
            if (wsMessage == null) return;
            if (wsMessage.Type == EWsMessageType.Heartbeat) return;

            string pattern = wsMessage.Pattern;
            OnReceived?.Invoke(connectData, pattern, msg);
        }
        public void SetMessageReceivedListener(Action<SClientConnectData, string, string> receivedCall)
        {
            OnReceived = receivedCall;
        }
        public void SendMessage(string clientId, string message)
        {
            if (!clients.ContainsKey(clientId)) return;
            WebSocket? ws = clients[clientId].WebSocket;

            if (ws != null && ws.State != WebSocketState.Open) return;

            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                ws?.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException wsex)
            {
                Console.WriteLine($"WebSocketException: {wsex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"其他异常: {ex.GetType().Name} - {ex.Message}");
            }
        }
        #endregion

        #region 心跳循环
        private void StartHeartbeatLoop()
        {
            heartbeatRunning = false;
            tickHandle = tickManager.RegisterTick(heartbeatIntervalMs, HeartbeatLoop);
        }
        private void StopHeartbeatLoop()
        {
            tickHandle?.Stop();
        }
        private async void HeartbeatLoop()
        {
            if (heartbeatRunning) return; // 避免重入

            try
            {
                heartbeatRunning = true;
                WebSocketResult<string> wsMessage = new WebSocketResult<string>();
                wsMessage.Type = EWsMessageType.Heartbeat;
                foreach (var kvp in clients.ToArray()) // 避免枚举时修改异常
                {
                    var clientId = kvp.Key;
                    var client = kvp.Value;
                    var token = client.Token;
                    var ws = client.WebSocket;

                    if (ws == null || ws.State != WebSocketState.Open)
                    {
                        string? account = tokenService.GetAccount(token);
                        Console.WriteLine($"[{account}] 状态异常 ({ws?.State})，移除");
                        RemoveClient(clientId);
                        continue;
                    }
                    // 检查超时
                    var diff = DateTime.UtcNow - client.LastActiveTime;
                    if (diff.TotalSeconds > 15)
                    {
                        string? account = tokenService.GetAccount(token);
                        HeartbeatTimeout(clientId);
                        RemoveClient(clientId);
                        Console.WriteLine($"[{account}] 心跳超时，关闭连接");

                        continue;
                    }
                    wsMessage.ServerFrame = frameManager.ServerCurrentFrame;
                    wsMessage.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    string heartbeatJson = JsonConvert.SerializeObject(wsMessage);
                    // 发送心跳包
                    SendMessage(clientId, heartbeatJson);
                }
            }
            finally
            {
                heartbeatRunning = false;
            }
        }
        private void ClientCloseConnect(string clientId)
        {
            OnConnStateChanged?.Invoke(
                new SClientConnectData
                {
                    ClientId = clientId,
                    Account = clients.ContainsKey(clientId) ? clients[clientId].Account : ""
                },
                EPlayerConnectionState.Closed
            );
        }
        private void HeartbeatTimeout(string clientId)
        {
            OnConnStateChanged?.Invoke(
                new SClientConnectData
                {
                    ClientId = clientId,
                    Account = clients.ContainsKey(clientId) ? clients[clientId].Account : ""
                },
                EPlayerConnectionState.HeartbeatTimeout
            );
        }
        #endregion
        private class ClientConnection
        {
            public string ClientId { get; set; } = "";
            public string Token { get; set; } = "";
            public string Account { get; set; } = "";
            public DateTime LastActiveTime { get; set; } = DateTime.UtcNow;
            public WebSocket? WebSocket { get; set; } = new ClientWebSocket();

            public CancellationTokenSource Cts = new();

            private int _closed = 0;

            public async Task SafeCloseAsync(WebSocketCloseStatus status, string reason)
            {
                if (Interlocked.Exchange(ref _closed, 1) == 1) return;

                try
                {
                    if (WebSocket != null &&
                        (WebSocket.State == WebSocketState.Open ||
                         WebSocket.State == WebSocketState.CloseReceived))
                    {
                        await WebSocket.CloseAsync(status, reason, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    // 服务端：日志要留，但不能让异常冒泡
                    Console.WriteLine($"WebSocket close failed: {ex.Message}");
                }
                finally
                {
                    WebSocket?.Dispose();
                }
            }
        }
    }
}
