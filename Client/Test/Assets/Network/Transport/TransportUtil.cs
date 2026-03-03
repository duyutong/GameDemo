using Network.Transport.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Network.Transport
{
    public static class TransportUtil
    {
        public static byte[] SerializeWsMessage<T>(WebSocketMessage<T> message)
        {
            if (GlobalSetting.Instance.format == ETransportFormat.Json)
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
        public static WebSocketResultHeader DeserializeWsResultHeader(byte[] bytes)
        {
            if (GlobalSetting.Instance.format == ETransportFormat.Json)
            {
                if (bytes.Length < 4) return null!;
                int jsonLength = BitConverter.ToInt32(bytes, 0);
                if (bytes.Length < 4 + jsonLength) return null!;

                byte[] jsonBytes = new byte[jsonLength];
                Array.Copy(bytes, 4, jsonBytes, 0, jsonLength);

                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonConvert.DeserializeObject<WebSocketResultHeader>(json)!;
            }
            else
            {
                var header = new WebSocketResultHeader();
                using var ms = new MemoryStream(bytes);
                using var reader = ProtoBuf.ProtoReader.Create(ms, ProtoBuf.Meta.RuntimeTypeModel.Default, null);
                int fieldNumber;
                while ((fieldNumber = reader.ReadFieldHeader()) > 0)
                {
                    switch (fieldNumber)
                    {
                        case 1: header.Code = reader.ReadInt32(); break;
                        case 2: header.Message = reader.ReadString(); break;
                        case 3: header.Pattern = reader.ReadString(); break;
                        case 4: header.Path = reader.ReadString(); break;
                        case 5: header.ServerFrame = reader.ReadInt32(); break;
                        case 6: header.Timestamp = reader.ReadInt64(); break;
                        default: reader.SkipField(); break;
                    }
                }
                return header;
            }
        }
        public static WebSocketResult<TData> DeserializeWsResult<TData>(byte[] bytes)
        {
            if (GlobalSetting.Instance.format == ETransportFormat.Json)
            {
                if (bytes.Length < 4) return null!;
                int jsonLength = BitConverter.ToInt32(bytes, 0);
                if (bytes.Length < 4 + jsonLength) return null!;

                byte[] jsonBytes = new byte[jsonLength];
                Array.Copy(bytes, 4, jsonBytes, 0, jsonLength);

                string json = Encoding.UTF8.GetString(jsonBytes);
                return JsonConvert.DeserializeObject<WebSocketResult<TData>>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<WebSocketResult<TData>>(ms);
            }
        }
    }
}
