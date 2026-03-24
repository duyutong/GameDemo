namespace FlexiServer.Sandbox.Interface
{
    public interface ISandboxEvent
    {
        void RegisterEvent(Action eventAction);
        void UnregisterEvent(Action eventAction);
    }
    public interface ISandboxEvent<T>
    {
        void RegisterEvent(Action<T> eventAction);
        void UnregisterEvent(Action<T> eventAction);
    }
    public interface ISandboxEvent<T1, T2>
    {
        void RegisterEvent(Action<T1, T2> eventAction);
        void UnregisterEvent(Action<T1, T2> eventAction);
    }
}
