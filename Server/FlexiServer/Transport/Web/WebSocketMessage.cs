namespace FlexiServer.Transport.Web
{
    public class WebSocketMessage<T>: TransportMessage
    {
        public EWsMessageType Type { get; set; } = EWsMessageType.Normal;
        public T? Data { get; set; }
        public int InputFrame { get; set; } = 0;
        public long Timestamp { get; set; }
    }
}
