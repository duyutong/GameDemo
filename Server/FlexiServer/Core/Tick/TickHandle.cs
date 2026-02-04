namespace FlexiServer.Core.Tick
{
    public class TickHandle(TickTask task, TickManager manager)
    {
        private readonly TickTask task = task;
        private readonly TickManager manager = manager;

        public void Stop()
        {
            manager.RemoveTick(task);
        }
    }
}
