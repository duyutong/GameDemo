using Newtonsoft.Json;
using System;
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
                HttpMessage<TReq> message = new()
                {
                    Account = NetworkManager.Instance.Account,
                    Data = req,
                };
                string json = JsonConvert.SerializeObject(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://{host}:{port}{path}", content);
                string resJson = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<HttpResult<TRes>>(resJson);
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
    }
}
