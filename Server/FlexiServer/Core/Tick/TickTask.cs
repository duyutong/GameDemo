namespace FlexiServer.Core.Tick
{
    public class TickTask
    {
        public int IntervalMs;
        public long NextRunAtMs;
        public Action Action;

        public TickTask(int intervalMs, Action action)
        {
            IntervalMs = intervalMs;
            NextRunAtMs = 0;
            Action = action;
        }
    }
}
