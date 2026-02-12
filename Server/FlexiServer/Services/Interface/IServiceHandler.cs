using FlexiServer.Core.Frame;
using FlexiServer.Sandbox;

namespace FlexiServer.Services.Interface
{
    public interface ISandboxUpdateHandler<TSandbox>
    {
        public void OnSandboxUpdate(TSandbox sandbox);
    }
    public interface ISandboxInitHandler<TSandbox>
    {
        public void OnSandboxInit(TSandbox sandbox);
    }
    public interface IFrameResolvedHandler
    {
        public void OnFrameResolved(int frame, List<FrameMessage> commands);
    }
}
