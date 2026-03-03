namespace FlexiServer.Transport.Http
{
    [global::ProtoBuf.ProtoContract(Name = @"HttpResult")]
    public class HttpResult<TData>
    {
        [global::ProtoBuf.ProtoMember(1)]
        public int Code { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Message { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public TData Data { get; set; }
    }
}