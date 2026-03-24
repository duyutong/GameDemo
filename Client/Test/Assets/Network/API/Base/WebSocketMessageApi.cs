using Network.Transport.WebSocket;
using System;
using System.Collections.Generic;

namespace Network.API
{
    public abstract class WebSocketMessageApi
    {
        public abstract string Pattern { get; set; }
        private readonly Dictionary<string, List<Action<WebSocketResult>>> listeners = new();
        public virtual void SendWebSocketMessage<T>(string pattern, string path, T messageObj)
        {
            NetworkManager.Instance.SendWebSocketMessage(pattern, path, messageObj);
        }
        public virtual void OnDataRecieved(string pattern, WebSocketResult result)
        {
            if (result == null) return;
            Dispatch(result);
        }
        private void Dispatch(WebSocketResult result)
        {
            string path = result.Path;
            if (string.IsNullOrEmpty(path)) return;
            if (!listeners.TryGetValue(path, out var list)) return;

            foreach (var callback in list) callback(result);
        }
        public virtual void AddListener(string path, Action<WebSocketResult> callback)
        {
            if (string.IsNullOrEmpty(path) || callback == null) return;

            if (!listeners.TryGetValue(path, out var list))
            {
                list = new List<Action<WebSocketResult>>();
                listeners[path] = list;
            }

            // 防止重复注册同一个 delegate 实例
            if (list.Contains(callback)) return;

            list.Add(callback);
        }

        public virtual void RemoveListener(string path, Action<WebSocketResult> callback)
        {
            if (string.IsNullOrEmpty(path) || callback == null) return;
            if (!listeners.TryGetValue(path, out var list)) return;

            list.Remove(callback);

            // 如果列表为空，删除整个 path
            if (list.Count == 0) listeners.Remove(path);
        }
    }
}