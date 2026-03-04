namespace FlexiServer.Transport.Http
{
    [global::ProtoBuf.ProtoContract(Name = @"HttpMessage")]
    public class HttpMessage
    {
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2)]
        public byte[]? Data { get; set; }
    }
}