namespace FlexiServer.Services.Interface
{
    public interface IImmediateHandler
    {
        void HandleImmediate(string clientId, string pattern, string command);
    }
}
