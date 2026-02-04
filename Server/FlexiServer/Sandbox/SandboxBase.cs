using FlexiServer.Core;

namespace FlexiServer.Sandbox
{
    public abstract class SandboxBase
    {
        public void Init<TSandBox>(Action<TSandBox> initAction)
            where TSandBox : SandboxBase
        {
            TSandBox self = (TSandBox)this;
            initAction?.Invoke(self);
        }

        public abstract void Reset();
        public abstract void Update();
        public abstract void Destroy();
    }
}