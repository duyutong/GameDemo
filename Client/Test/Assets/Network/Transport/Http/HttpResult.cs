namespace Network.Transport.Http
{
    [global::ProtoBuf.ProtoContract(Name = @"HttpResult")]
    public class HttpResult<T>
    {
        [global::ProtoBuf.ProtoMember(1)]
        public int Code { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Message { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public T Data { get; set; }
    }
}