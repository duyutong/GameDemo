using FlexiServer.Core;
using FlexiServer.Transport.Http;
using FlexiServer.Transport.Interface;
using FlexiServer.Transport.Udp;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;
using System.Text;

namespace FlexiServer.Transport
{
    public static class TransportUtil
    {
        public static HttpResult<TData> DeserializeHttpResult<TData>(byte[] bytes)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<HttpResult<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<HttpResult<TData>>(ms)!;
            }
        }
        public static byte[] SerializeHttpMessage<TData>(HttpMessage<TData> message)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = JsonConvert.SerializeObject(message);
                return Encoding.UTF8.GetBytes(json);
            }
            else
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }
        public static HttpMessage<TData> DeserializeHttpMessage<TData>(byte[] bytes)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<HttpMessage<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<HttpMessage<TData>>(ms)!;
            }
        }
        public static byte[] SerializeHttpResult<TData>(HttpResult<TData> message)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = JsonConvert.SerializeObject(message);
                return Encoding.UTF8.GetBytes(json);
            }
            else
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }
        public static UdpMessage<TData> DeserializeUdpMessage<TData>(byte[] bytes)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<UdpMessage<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<UdpMessage<TData>>(ms)!;
            }
        }
        public static byte[] SerializeUdpResult<TData>(UdpResult<TData> message)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = JsonConvert.SerializeObject(message);
                return Encoding.UTF8.GetBytes(json);
            }
            else
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }
        public static WebSocketMessage<TData> DeserializeWsMessage<TData>(byte[] bytes)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                if (bytes.Length < 4) return null!; // 处理空数据的情况，避免 JSON 解析错误

                int jsonLength = BitConverter.ToInt32(bytes, 0); // 读取前4字节长度

                if (bytes.Length < 4 + jsonLength) return null!; // 数据长度不足，可能是传输不完整

                byte[] jsonBytes = new byte[jsonLength];
                Array.Copy(bytes, 4, jsonBytes, 0, jsonLength);

                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonConvert.DeserializeObject<WebSocketMessage<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<WebSocketMessage<TData>>(ms)!;
            }
        }
        public static byte[] SerializeWsResult<TData>(WebSocketResult<TData> message)
        {
            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = JsonConvert.SerializeObject(message);
                return Encoding.UTF8.GetBytes(json);
            }
            else
            {
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, message);
                return ms.ToArray();
            }
        }
    }
}
