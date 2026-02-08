namespace FlexiServer.Transport.Http
{
    public class HttpMessage<T>
    {
        public string Account { get; set; } = "";
        public T? Data { get; set; }
    }
}
