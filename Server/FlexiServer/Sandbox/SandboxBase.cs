using FlexiServer.Core;

namespace FlexiServer.Sandbox
{
    public abstract class SandboxBase()
    {
        public Action<SandboxBase>? OnReleaseAction;
        public void Init<TSandBox>(Action<TSandBox>? initAction)
            where TSandBox : SandboxBase
        {
            TSandBox self = (TSandBox)this;
            initAction?.Invoke(self);
            OnInit();
        }
        public void Release() { OnReleaseAction?.Invoke(this); }
        public abstract void OnInit();
        public abstract void OnReset();
        public abstract void OnUpdate();
        public abstract void OnDestroy();
    }
}