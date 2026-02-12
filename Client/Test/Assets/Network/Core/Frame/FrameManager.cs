using Network.Core.Tick;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Network.Core.Frame
{
    public class FrameManager
    {
        public static FrameManager Instance { get; private set; } = new FrameManager();
        // 当前权威帧
        public int ServerCurrentFrame { get; private set; }
        // 本地帧
        public int LocalCurrentFrame { get; private set; }
        // 帧同步刷新间隔:50ms
        public int FrameSyncIntervalMs { get; set; } = 50;
        // 帧更新
        private TickHandle tickHandle;
        // 当前本地帧对应的时间点
        private DateTime currentFrameTime;
        // 服务器最近推送的服务器时间戳（毫秒）
        private long LastServerSendTimestamp = 0;
        // 记录客户端接收到服务器消息的本地时间（毫秒）
        private long lastReceiveLocalTimestampMs = 0;
        
        private float smoothRTT = 0;
        public void StartFrameLoop()
        {
            tickHandle = TickManager.Instance.RegisterTick(FrameSyncIntervalMs, FrameLoop);
        }
        public void StopFrameLoop()
        {
            tickHandle?.Stop();
        }
        private void FrameLoop()
        {
            currentFrameTime = DateTime.UtcNow;
            LocalCurrentFrame++;

            SmoothFrameSync();
        }
        public void RefreshServerFrame(int serverFrame, long serverSendTimestamp)
        {
            ServerCurrentFrame = serverFrame;
            LastServerSendTimestamp = serverSendTimestamp;

            if (LocalCurrentFrame == 0) LocalCurrentFrame = serverFrame;

            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            // 单次RTT估算（消息到达时间 - 消息发送时间）
            long rtt = now - serverSendTimestamp;
            // 平滑 RTT (EMA)
            smoothRTT = (smoothRTT * 0.8f) + (rtt * 0.2f);

            lastReceiveLocalTimestampMs = now;
        }
        private void SmoothFrameSync()
        {
            if (LastServerSendTimestamp == 0) return;

            // 正确的“服务器帧推进时间” = 本地当前时间 - 本地收到消息时的时间 + 已知网络延迟的一半
            long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long deltaSinceReceive = now - lastReceiveLocalTimestampMs;
            long deltaMs = deltaSinceReceive + (long)(smoothRTT * 0.5f);

            // 服务器此刻应该在的帧
            float framesPassed = deltaMs / (float)FrameSyncIntervalMs;

            int estimatedServerNowFrame = ServerCurrentFrame + Mathf.RoundToInt(framesPassed);
            int diff = estimatedServerNowFrame - LocalCurrentFrame;

            if (Mathf.Abs(diff) > 5)
            {
                LocalCurrentFrame = estimatedServerNowFrame;
                return;
            }

            if (diff > 0)
            {
                int catchUp = Mathf.Clamp(diff, 1, 3); // 最多每帧追 3 帧
                LocalCurrentFrame += catchUp;
            }
        }
    }
}
