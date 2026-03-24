using Assets.Network.Transport;
using System;
using System.Net.Http;
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
        public async Task<HttpResult> PostAsync<TReq, TRes>(string path, TReq req)
        {
            try
            {
                HttpMessage message = new();
                message.Account = NetworkManager.Instance.Account;
                message.Data = req.ToBytes();

                HttpContent content = GetHttpContent(message);
                var response = await client.PostAsync($"http://{host}:{port}{path}", content);
                byte[] resBytes = await response.Content.ReadAsByteArrayAsync();

                return resBytes.ConvertData<HttpResult>();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new HttpResult
                {
                    Code = -1,
                    Message = ex.Message,
                    Data = default
                };
            }
        }
        private HttpContent GetHttpContent(HttpMessage message)
        {
            HttpContent content;
            byte[] bytes = message.ToBytes();
            content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");

            return content;
        }
    }
}
