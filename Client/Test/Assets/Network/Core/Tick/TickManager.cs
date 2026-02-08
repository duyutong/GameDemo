using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
namespace Network.Core.Tick
{
    public class TickManager
    {
        public static TickManager Instance { get; private set; } = new TickManager();
        private readonly List<TickTask> ticks = new();
        private readonly CancellationTokenSource cts = new();
        private readonly Stopwatch sw = Stopwatch.StartNew();
        private Task loopTask;
        public void StartTickLoop()
        {
            if (loopTask != null) return; // 避免重复开启
            loopTask = Task.Run(() => MainLoop(cts.Token));
        }

        public void StopTickLoop() => cts.Cancel();

        // 注册 Tick：intervalMs 毫秒执行一次 action
        public TickHandle RegisterTick(int intervalMs, Action action)
        {
            var tick = new TickTask(intervalMs, action);
            lock (ticks) ticks.Add(tick);
            return new TickHandle(tick, this);
        }

        public void RemoveTick(TickTask task)
        {
            lock (ticks) ticks.Remove(task);
        }

        private async Task MainLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                long now = sw.ElapsedMilliseconds;

                List<TickTask> snapshot;// 复制一份当前的 Tick 列表
                lock (ticks) snapshot = new List<TickTask>(ticks);

                foreach (var t in snapshot)
                {
                    if(t.NextTick == 0) t.NextTick = now;
                    if (now >= t.NextTick)
                    {
                        t.NextTick += t.IntervalMs;
                        if (t.NextTick < now) t.NextTick = now + t.IntervalMs;

                        try { t.Action?.Invoke(); }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Tick error: {ex}");
                        }
                    }
                }

                // 主循环以 1ms 精度睡眠
                await Task.Delay(1, token);
            }
        }
    }
}