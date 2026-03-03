namespace FlexiServer.Transport.Web
{
    [global::ProtoBuf.ProtoContract(Name = @"WebSocketMessage")]
    public class WebSocketMessage<TData>
    {
        [global::ProtoBuf.ProtoMember(1)]
        public string Pattern { get; set; } = "";
        [global::ProtoBuf.ProtoMember(2)]
        public string Path { get; set; } = "";

        [global::ProtoBuf.ProtoMember(3)]
        public long Timestamp { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int InputFrame { get; set; } = 0;
        
        [global::ProtoBuf.ProtoMember(5)]
        public TData? Data { get; set; }
    }

    [global::ProtoBuf.ProtoContract(Name = @"WebSocketMessageHeader")]
    public class WebSocketMessageHeader
    {
        [global::ProtoBuf.ProtoMember(1)]
        public string Pattern { get; set; } = "";
        [global::ProtoBuf.ProtoMember(2)]
        public string Path { get; set; } = "";

        [global::ProtoBuf.ProtoMember(3)]
        public long Timestamp { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int InputFrame { get; set; } = 0;
    }
}