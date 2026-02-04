namespace FlexiServer.Services.Interface
{
    public interface IFrameHandler
    {
        void HandleFrame(int inputFrame, string clientId, string pattern, string command);
    }
}
