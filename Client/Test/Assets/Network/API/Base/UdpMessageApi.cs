using Network;
using Network.Transport.Udp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Network.API
{
    public abstract class UdpMessageApi
    {
        public abstract string Pattern { get; set; }
        public int currentFrame;//当前操作到哪一帧（对应服务器发出操作时赋值的权威帧）
        private Dictionary<string, Dictionary<Delegate, Action<UdpResult<object>>>> listeners = new();
        public void SendUdpMessage<TSend>(string pattern,string path, TSend messageObj)
        {
            NetworkManager.Instance.SendUdpMessage(pattern, path, messageObj);
        }
        public virtual void OnDataRecieved(string pattern, string msg) 
        {
            UdpResult<object> objResult = JsonConvert.DeserializeObject<UdpResult<object>>(msg);
            if (objResult == null) return;
            if (objResult.ServerFrame < currentFrame) return;
            currentFrame = objResult.ServerFrame;

            Dispatch(objResult.Path, objResult);
        }
        private void Dispatch(string path, UdpResult<object> result)
        {
            if (listeners.TryGetValue(path, out var map))
            {
                foreach (var wrapper in map.Values)
                    wrapper(result);
            }
        }
        public virtual void AddListener<T>(string path, Action<UdpResult<T>> callBack)
        {
            if (callBack == null) return;

            if (!listeners.TryGetValue(path, out var map))
            {
                map = new Dictionary<Delegate, Action<UdpResult<object>>>();
                listeners[path] = map;
            }

            // 已经注册过，直接 return
            if (map.ContainsKey(callBack)) return;

            Action<UdpResult<object>> wrapper = (objResult) =>
            {
                var real = new UdpResult<T>
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
        public virtual void RemoveListener<T>(string path, Action<UdpResult<T>> callBack)
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