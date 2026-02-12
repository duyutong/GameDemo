using FlexiServer.Core.Tick;
using System.Collections.Concurrent;
namespace FlexiServer.Core.Frame
{
    public class FrameManager(TickManager ticks)
    {
        public FrameManager Instance => this;
        // 当前权威帧
        public int ServerCurrentFrame { get; private set; }
        // 最大回滚帧数
        private int MaxRollbackFrames { get; set; } = 10;
        // 帧同步刷新间隔:50ms
        public int FrameSyncIntervalMs { get; set; } = 50;
        // 最大回滚帧数
        private int rollbackStartFrame = 0;
        // 当前权威帧对应的时间点
        private DateTime currentFrameTime;
        // 消息队列循环
        private readonly ConcurrentDictionary<int, ConcurrentQueue<FrameMessage>> messagePool = new();
        private ConcurrentDictionary<int, ConcurrentQueue<FrameMessage>> rollbackQuene = new();
        // 帧更新
        private TickHandle? tickHandle;
        // 回滚后帧管理器返回给 Service 的回调函数
        public Action<int, List<FrameMessage>>? OnFrameResolved;
        public void AddFrameMessageToPool(int inputFrame,string clientId, string pattern,string path, string command)
        {
            if (inputFrame < ServerCurrentFrame - MaxRollbackFrames) return;
            rollbackStartFrame = Math.Min(rollbackStartFrame, inputFrame);

            var msg = FrameMessagePool.Get();
            msg.Pattern = pattern;
            msg.Path = path;
            msg.InputFrame = inputFrame;
            msg.ClientId = clientId;
            msg.Command = command;

            // 获取或创建对应帧的消息队列
            var queue = messagePool.GetOrAdd(
                msg.InputFrame,
                _ => new ConcurrentQueue<FrameMessage>()
            );
            // 添加消息到队列
            queue.Enqueue(msg);
        }
        public void StartFrameLoop()
        {
            tickHandle = ticks.RegisterTick(FrameSyncIntervalMs, FrameLoop);
        }
        public void StopFrameLoop()
        {
            OnFrameResolved = null;
            messagePool.Clear();
            tickHandle?.Stop();
        }
        private void ClearMessagePool(int frame) 
        {
            messagePool.TryGetValue(frame, out var msg);
            if (msg == null) { messagePool.TryRemove(frame,out _); return; }

            foreach (var frameMsg in msg) FrameMessagePool.Return(frameMsg);
        }
        private void FrameLoop()
        {
            rollbackQuene.Clear();
            // 处理消息
            var currFrame = ServerCurrentFrame;
            currentFrameTime = DateTime.UtcNow;
            ServerCurrentFrame++;
            for (int i = rollbackStartFrame; i < currFrame; i++)
            {
                int checkFrame = i;
                if (messagePool.TryGetValue(checkFrame, out var queue))
                {
                    foreach (var msg in queue)
                    {
                        if (msg == null) continue;
                        rollbackQuene.AddOrUpdate(
                            checkFrame,
                            _ =>
                            {
                                var newQueue = new ConcurrentQueue<FrameMessage>();
                                newQueue.Enqueue(msg);
                                return newQueue;
                            },
                            (_, existingQueue) =>
                            {
                                existingQueue.Enqueue(msg);
                                return existingQueue;
                            }
                        );
                    }
                    
                    OnFrameResolved?.Invoke(checkFrame, [.. rollbackQuene[checkFrame]]);
                }
            }

            ClearMessagePool(currFrame - MaxRollbackFrames);
            rollbackStartFrame = currFrame;
        }
    }
}
