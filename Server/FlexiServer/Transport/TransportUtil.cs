using FlexiServer.Core;
using FlexiServer.Transport.Http;
using Newtonsoft.Json;
using System.Text;

namespace FlexiServer.Transport
{
    public static class TransportUtil
    {
        /// <summary>
        /// 将对象序列化为 byte[]，自动处理 JSON 或 Protobuf
        /// JSON 会带 4 字节长度前缀，Protobuf 直接写入
        /// </summary>
        public static byte[] ToBytes<TData>(this TData data)
        {
            if (data == null) return Array.Empty<byte>();

            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                // JSON 序列化
                string json = JsonConvert.SerializeObject(data);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

                // 前 4 字节写长度
                byte[] result = new byte[4 + jsonBytes.Length];
                BitConverter.GetBytes(jsonBytes.Length).CopyTo(result, 0);
                Buffer.BlockCopy(jsonBytes, 0, result, 4, jsonBytes.Length);

                return result;
            }
            else
            {
                // Protobuf 序列化
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, data);
                byte[] protoBytes = ms.ToArray();

                // 给 Protobuf 也加长度前缀，更安全
                byte[] result = new byte[4 + protoBytes.Length];
                BitConverter.GetBytes(protoBytes.Length).CopyTo(result, 0);
                Buffer.BlockCopy(protoBytes, 0, result, 4, protoBytes.Length);

                return result;
            }
        }

        /// <summary>
        /// 将 byte[] 反序列化为对象，自动处理 JSON 或 Protobuf
        /// JSON 和 Protobuf 都假设前 4 字节是长度
        /// </summary>
        public static TData? ConvertData<TData>(this byte[] bytes) where TData : class
        {
            if (bytes == null || bytes.Length < 4) return null;

            int length = BitConverter.ToInt32(bytes, 0);
            if (bytes.Length < 4 + length) return null;

            byte[] dataBytes = new byte[length];
            Array.Copy(bytes, 4, dataBytes, 0, length);

            if (GlobalSetting.Format == ETransportFormat.Json)
            {
                string json = Encoding.UTF8.GetString(dataBytes);
                return JsonConvert.DeserializeObject<TData>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(dataBytes);
                return ProtoBuf.Serializer.Deserialize<TData>(ms);
            }
        }
    }
}
