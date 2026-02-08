using Network.Transport.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Network.API
{
    public abstract class WebSocketMessageApi
    {
        public abstract string Pattern { get; set; }
        private Dictionary<string, Dictionary<Delegate, Action<WebSocketResult<object>>>> listeners = new();
        public virtual void SendWebSocketMessage<T>(string pattern, string path, T messageObj)
        {
            NetworkManager.Instance.SendWebSocketMessage(pattern, path, messageObj);
        }
        public virtual void OnDataRecieved(string pattern, string msg)
        {
            WebSocketResult<object> objResult = JsonConvert.DeserializeObject<WebSocketResult<object>>(msg);
            if (objResult == null) return;
            Dispatch(objResult.Path, objResult);
        }
        public void Dispatch(string path, WebSocketResult<object> result)
        {
            if (listeners.TryGetValue(path, out var map))
            {
                foreach (var wrapper in map.Values)
                    wrapper(result);
            }
        }

        protected float GetDeltaSeconds(WebSocketResult<object> wsMessage)
        {
            long currTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long inputTime = wsMessage.Timestamp;
            float deltaSeconds = (currTime - inputTime) / 1000f;
            return deltaSeconds;
        }
        public virtual void AddListener<T>(string path, Action<WebSocketResult<T>> callBack)
        {
            if (callBack == null) return;

            if (!listeners.TryGetValue(path, out var map))
            {
                map = new Dictionary<Delegate, Action<WebSocketResult<object>>>();
                listeners[path] = map;
            }

            // 已经注册过，直接 return
            if (map.ContainsKey(callBack)) return;

            Action<WebSocketResult<object>> wrapper = (objResult) =>
            {
                var real = new WebSocketResult<T>
                {
                    Code = objResult.Code,
                    Message = objResult.Message,
                    ServerFrame = objResult.ServerFrame,
                    Timestamp = objResult.Timestamp,
                    Path = objResult.Path,
                    Pattern = objResult.Pattern,
                    Data = ConvertData<T>(objResult.Data)
                };

                callBack(real);
            };

            map[callBack] = wrapper;
        }

        public virtual void RemoveListener<T>(string path, Action<WebSocketResult<T>> callBack)
        {
            if (callBack == null) return;
            if (!listeners.TryGetValue(path, out var map)) return;
            if (!map.Remove(callBack)) return;

            if (map.Count == 0) listeners.Remove(path);
        }
        private T ConvertData<T>(object data)
        {
            if (data == null) return default;

            if (data is T value) return value;

            if (data is JToken token)
                return token.ToObject<T>();

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
        }
    }
}