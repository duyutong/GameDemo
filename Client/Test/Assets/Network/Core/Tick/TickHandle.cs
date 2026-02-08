namespace Network.Core.Tick
{
    public class TickHandle
    {
        private readonly TickTask task;
        private readonly TickManager manager;

        public TickHandle(TickTask task, TickManager manager)
        {
            this.task = task;
            this.manager = manager;
        }

        public void Stop()
        {
            manager.RemoveTick(task);
        }
    }
}
