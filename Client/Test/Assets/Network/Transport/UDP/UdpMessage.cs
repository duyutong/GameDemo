using UnityEngine;

namespace Network.Transport.Udp
{
    [global::ProtoBuf.ProtoContract(Name = @"UdpMessage")]
    public class UdpMessage
    {
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2)]
        public string Pattern { get; set; } = "";

        [global::ProtoBuf.ProtoMember(3)]
        public string Path { get; set; } = "";

        [global::ProtoBuf.ProtoMember(4)]
        public byte[] Data { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int InputFrame { get; set; } = 0;

        [global::ProtoBuf.ProtoMember(6)]
        public long Timestamp { get; set; }
    }
}