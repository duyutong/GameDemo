using Network.Transport.Udp;
using System;
using System.Collections.Generic;

namespace Network.API
{
    public abstract class UdpMessageApi
    {
        public abstract string Pattern { get; set; }
        public int currentFrame;//当前操作到哪一帧（对应服务器发出操作时赋值的权威帧）
        private readonly Dictionary<string, List<Action<UdpResult>>> listeners = new();
        public void SendUdpMessage<TSend>(string pattern, string path, TSend messageObj)
        {
            NetworkManager.Instance.SendUdpMessage(pattern, path, messageObj);
        }
        public virtual void OnDataRecieved(string pattern, UdpResult result)
        {
            if (result == null) return;
            currentFrame = result.ServerFrame;

            Dispatch(result);
        }
        private void Dispatch(UdpResult result)
        {
            string path = result.Path;
            if (string.IsNullOrEmpty(path)) return;
            if (!listeners.TryGetValue(path, out var list)) return;

            foreach (var callback in list) callback(result);
        }
        public virtual void AddListener(string path, Action<UdpResult> callback)
        {
            if (callback == null) return;

            if (!listeners.TryGetValue(path, out var list))
            {
                list = new List<Action<UdpResult>>();
                listeners[path] = list;
            }

            // 防止重复注册同一个 delegate 实例
            if (list.Contains(callback)) return;

            list.Add(callback);
        }
        public virtual void RemoveListener(string path, Action<UdpResult> callback)
        {
            if (string.IsNullOrEmpty(path) || callback == null) return;
            if (!listeners.TryGetValue(path, out var list)) return;

            list.Remove(callback);

            // 如果列表为空，删除整个 path
            if (list.Count == 0) listeners.Remove(path);
        }
    }
}