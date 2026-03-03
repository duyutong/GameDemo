namespace Network.Transport.WebSocket
{
    [global::ProtoBuf.ProtoContract(Name = @"WebSocketResult")]
    public class WebSocketResult<TData>
    {
        [global::ProtoBuf.ProtoMember(1)]
        public int Code { get; set; } = 200;

        [global::ProtoBuf.ProtoMember(2)]
        public string Message { get; set; } = "succ";

        [global::ProtoBuf.ProtoMember(3)]
        public string Pattern { get; set; } = "";

        [global::ProtoBuf.ProtoMember(4)]
        public string Path { get; set; } = "";

        [global::ProtoBuf.ProtoMember(5)]
        public int ServerFrame { get; set; } = 0;

        [global::ProtoBuf.ProtoMember(6)]
        public long Timestamp { get; set; }

        [global::ProtoBuf.ProtoMember(7)]
        public TData Data { get; set; }
    }

    [global::ProtoBuf.ProtoContract(Name = @"WebSocketResultHeader")]
    public class WebSocketResultHeader
    {
        [global::ProtoBuf.ProtoMember(1)]
        public int Code { get; set; } = 200;

        [global::ProtoBuf.ProtoMember(2)]
        public string Message { get; set; } = "succ";

        [global::ProtoBuf.ProtoMember(3)]
        public string Pattern { get; set; } = "";

        [global::ProtoBuf.ProtoMember(4)]
        public string Path { get; set; } = "";

        [global::ProtoBuf.ProtoMember(5)]
        public int ServerFrame { get; set; } = 0;

        [global::ProtoBuf.ProtoMember(6)]
        public long Timestamp { get; set; }
    }
}