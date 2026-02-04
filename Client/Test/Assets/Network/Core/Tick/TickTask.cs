using System;

namespace Network.Core.Tick
{
    public class TickTask
    {
        public int IntervalMs;
        public long NextTick;
        public Action Action;

        public TickTask(int intervalMs, Action action)
        {
            IntervalMs = intervalMs;
            NextTick = 0;
            Action = action;
        }
    }
}
