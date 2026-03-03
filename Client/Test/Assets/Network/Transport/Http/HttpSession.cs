using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.Transport.Http
{
    public class HttpSession : ConnectionSession
    {
        private HttpClient client = new HttpClient();
        public HttpSession(string host, int port) : base(host, port)
        {
            this.host = host;
            this.port = port;
        }
        public async Task<HttpResult<TRes>> PostAsync<TReq, TRes>(string path, TReq req)
        {
            try
            {
                HttpMessage<TReq> message = new();
                message.Account = NetworkManager.Instance.Account;
                message.Data = req;

                HttpContent content = GetHttpContent(message);
                var response = await client.PostAsync($"http://{host}:{port}{path}", content);
                byte[] resBytes = await response.Content.ReadAsByteArrayAsync();

                return DeserializeHttpResult<TRes>(resBytes);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                return new HttpResult<TRes>
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = default
                };
            }
        }
        private HttpResult<TRes> DeserializeHttpResult<TRes>(byte[] resBytes)
        {
            if (resBytes.Length == 0) return null!;
            // 判断流是不是 JSON
            if (resBytes[0] == (byte)'{') // JSON 一般以 { 开头
            {
                string resJson = Encoding.UTF8.GetString(resBytes);
                return JsonConvert.DeserializeObject<HttpResult<TRes>>(resJson)!;
            }
            else
            {
                using var ms = new MemoryStream(resBytes);
                return ProtoBuf.Serializer.Deserialize<HttpResult<TRes>>(ms);
            }
        }
        private HttpContent GetHttpContent<TReq>(HttpMessage<TReq> message)
        {
            HttpContent content;
            if (GlobalSetting.Instance.format == ETransportFormat.Protobuf)
            {
                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(ms, message);
                    bytes = ms.ToArray();
                }
                content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");
            }
            else
            {
                string json = JsonConvert.SerializeObject(message);
                content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return content;
        }
    }
}
