using UnityEngine;

namespace Network.Transport.Udp
{
    public class UdpMessage<T>
    {
        public string Account { get; set; } = "";
        public string Pattern { get; set; } = "";
        public string Path { get; set; } = "";
        public T Data { get; set; }
        public int InputFrame { get; set; } = 0;
        public long Timestamp { get; set; }
    }
}