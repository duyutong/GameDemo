namespace FlexiServer.Transport.Udp
{
    public class UdpResult<T>
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "succ";
        public string Pattern { get; set; } = "";
        public string Path { get; set; } = "";
        public T? Data { get; set; }
        public int ServerFrame { get; set; } = 0;
        public long Timestamp { get; set; }
    }
}
