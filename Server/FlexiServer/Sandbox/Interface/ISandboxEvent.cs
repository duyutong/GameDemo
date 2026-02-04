namespace FlexiServer.Sandbox.Interface
{
    public interface ISandboxEvent<T>
    {
        void RegisterEvent(Action<T> eventAction);
        void UnregisterEvent(Action<T> eventAction);
    }
}
