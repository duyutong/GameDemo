namespace FlexiServer.Transport.Web
{
    public class WebSocketMessage<T>
    {
        public EWsMessageType Type { get; set; } = EWsMessageType.Normal;
        public string Pattern { get; set; } = "";
        public string Path { get; set; } = "";
        public T? Data { get; set; }
        public int InputFrame { get; set; } = 0;
        public long Timestamp { get; set; }
    }
}
