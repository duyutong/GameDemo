using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Transport.Http;
using FlexiServer.Transport.Interface;
using FlexiServer.Transport.Udp;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;
using System;
using System.Text;

namespace FlexiServer.Transport
{
    public static class TransportUtil
    {
        public static async void ReturnHttpResultTask<TRes>(HttpContext context, HttpResult<TRes> result) 
        {
            ETransportFormat format = CheckTransportFormat(context);
            if (format == ETransportFormat.Json)
            {
                // 返回 JSON
                await context.Response.WriteAsJsonAsync(result);
            }
            else
            {
                // 返回 protobuf
                context.Response.ContentType = "application/x-protobuf";

                // 使用 MemoryStream + 同步 Serializer.Serialize
                using var ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize(ms, result);

                ms.Position = 0;
                await ms.CopyToAsync(context.Response.Body);
            }
        }
        public static ETransportFormat CheckTransportFormat(HttpContext context) 
        {
            if (context.Request.ContentType?.Contains("application/x-protobuf") == true)return ETransportFormat.Protobuf;
            return ETransportFormat.Json;
        }
        public static async Task<HttpMessage<TReq>> ReadHttpMessageAsync<TReq>(HttpContext context) 
        {
            HttpMessage<TReq> msg;
            // 根据 Content-Type 判断
            if (context.Request.ContentType?.Contains("application/x-protobuf") == true)
            {
                using var ms = new MemoryStream();
                await context.Request.Body.CopyToAsync(ms);
                ms.Position = 0;

                msg = ProtoBuf.Serializer.Deserialize<HttpMessage<TReq>>(ms);
            }
            else
            {
                msg = await context.Request.ReadFromJsonAsync<HttpMessage<TReq>>();
            }
            return msg!;
        }
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
        public static UdpMessageHeader DeserializeUdpMessageHeader(byte[] bytes) 
        {
            if(GlobalSetting.Format == ETransportFormat.Json) 
            {
                var json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<UdpMessageHeader>(json)!;
            }
            else
            {
                using var ms = new MemoryStream(bytes);
                return ProtoBuf.Serializer.Deserialize<UdpMessageHeader>(ms);
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
        public static WebSocketMessageHeader DeserializeWsMessageHeader(byte[] bytes) 
        {
            if (GlobalSetting.Format == ETransportFormat.Json) 
            {
                var json = Encoding.UTF8.GetString(bytes);
                return JsonConvert.DeserializeObject<WebSocketMessageHeader>(json)!;
            }
            else
            {
                var header = new WebSocketMessageHeader();
                using var ms = new MemoryStream(bytes);
                using var reader = ProtoBuf.ProtoReader.Create(ms, ProtoBuf.Meta.RuntimeTypeModel.Default, null);
                int fieldNumber;
                while ((fieldNumber = reader.ReadFieldHeader()) > 0) 
                {
                    switch (fieldNumber) 
                    {
                        case 1: header.Pattern = reader.ReadString(); break;
                        case 2: header.Path = reader.ReadString(); break;
                        case 3: header.Timestamp = reader.ReadInt64(); break;
                        case 4: header.InputFrame = reader.ReadInt32(); break;
                        default:  reader.SkipField(); break;
                    }
                }
                return header;
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
