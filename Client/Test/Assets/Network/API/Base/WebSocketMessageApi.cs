using Assets.Network.Transport;
using Network.Transport.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

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
        public virtual void OnDataRecieved(string pattern, byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return;
            Dispatch(buffer);
        }
        private void Dispatch(byte[] buffer)
        {
            string path = TransportUtil.DeserializeWsResultHeader(buffer)?.Path;
            if (listeners.TryGetValue(path, out var map))
            {
                foreach (var wrapper in map.Values)
                    wrapper(buffer);
            }
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

            Action<byte[]> wrapper = (buffer) =>
            {
                WebSocketResult<T> objResult = TransportUtil.DeserializeWsResult<T>(buffer);
                //var real = new WebSocketResult<T>
                //{

                //    Code = objResult.Code,
                //    Message = objResult.Message,
                //    ServerFrame = objResult.ServerFrame,
                //    Timestamp = objResult.Timestamp,
                //    Path = objResult.Path,
                //    Pattern = objResult.Pattern,
                //    Data = ConvertData<T>(objResult.Data)
                //};

                callBack(objResult);
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