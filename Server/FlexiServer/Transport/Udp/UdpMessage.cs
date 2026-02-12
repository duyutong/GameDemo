namespace FlexiServer.Transport.Udp
{
    public class UdpMessage<T>:TransportMessage
    {
        public string Account { get; set; } = "";
        public T? Data { get; set; }
        public int InputFrame { get; set; } = 0;
        public long Timestamp { get; set; }
    }
}
