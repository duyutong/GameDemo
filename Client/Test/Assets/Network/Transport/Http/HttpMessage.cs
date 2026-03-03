namespace Network.Transport.Http
{
    [global::ProtoBuf.ProtoContract(Name = @"HttpMessage")]
    public class HttpMessage<T>
    {
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2)]
        public T Data { get; set; }
    }
}