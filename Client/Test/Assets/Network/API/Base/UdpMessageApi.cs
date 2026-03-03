using Network;
using Network.Transport.Udp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Network.API
{
    public abstract class UdpMessageApi
    {
        public abstract string Pattern { get; set; }
        public int currentFrame;//当前操作到哪一帧（对应服务器发出操作时赋值的权威帧）
        private Dictionary<string, Dictionary<Delegate, Action<UdpResult<object>>>> listeners = new();
        public void SendUdpMessage<TSend>(string pattern, string path, TSend messageObj)
        {
            NetworkManager.Instance.SendUdpMessage(pattern, path, messageObj);
        }
        public virtual void OnDataRecieved(string pattern, UdpResult<object> result)
        {
            if (result == null) return;
            if (result.ServerFrame < currentFrame) return;
            currentFrame = result.ServerFrame;

            Dispatch(result.Path, result);
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

            if (GlobalSetting.Instance.format == ETransportFormat.Protobuf)
            {
                if (data is byte[] bytes)
                {
                    using var ms = new MemoryStream(bytes);
                    return ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                else if (data is MemoryStream ms)
                {
                    return ProtoBuf.Serializer.Deserialize<T>(ms);
                }
                else
                {
                    // 如果 data 已经是对象类型，尝试先序列化再反序列化
                    using var tmpMs = new MemoryStream();
                    ProtoBuf.Serializer.Serialize(tmpMs, data);
                    tmpMs.Position = 0;
                    return ProtoBuf.Serializer.Deserialize<T>(tmpMs);
                }
            }
            else
            {
                // JSON: data 可能是 JObject / JToken / 原生对象
                if (data is JToken token) return token.ToObject<T>();
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
            }
        }
    }
}