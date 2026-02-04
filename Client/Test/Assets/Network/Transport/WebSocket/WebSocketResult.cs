namespace Network.Transport.WebSocket
{
    public class WebSocketResult<T>
    {
        public EWsMessageType Type { get; set; } = EWsMessageType.Normal;
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "succ";
        public string Pattern { get; set; } = "";
        public string Path { get; set; } = "";
        public int ServerFrame { get; set; } = 0;
        public long Timestamp { get; set; }
        public T Data { get; set; }
    }
}
