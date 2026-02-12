using FlexiServer.Core;
using FlexiServer.Services;

namespace FlexiServer.Sandbox
{
    public class SandboxBase()
    {
        public int SandboxId { get; set; }
        public int LastSandboxFramerate { get; set; } = 0;
        public Action<SandboxBase>? OnReleaseAction;
        public void Init<TSandBox>(Action<TSandBox>? initAction)
            where TSandBox : SandboxBase
        {
            TSandBox self = (TSandBox)this;
            initAction?.Invoke(self);
            OnInit();
        }
        public bool HasSendableUpdate(int serverCurrentFrame, int maxFrameLag = 10)
        {
            return Math.Abs(serverCurrentFrame - LastSandboxFramerate) <= maxFrameLag;
        }
        public void Release() { OnReleaseAction?.Invoke(this); }
        public virtual void OnInit() { }
        public virtual void OnReset() { }
        public virtual void OnUpdate() { }
        public virtual void OnDestroy() { }
    }
}