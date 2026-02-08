using FlexiServer.Models.Common;
using FlexiServer.Transport.Http;
using Newtonsoft.Json;
using System.Text;

namespace FlexiServer.Infrastructure.InternalServices
{
    public class InternalServiceClient(IConfiguration config)
    {
        private HttpClient client = new HttpClient();
        public async Task<HttpResult<TRes>> PostAsync<TReq, TRes>(string role, string path, TReq req)
        {
            string url = $"{GetUrlByRole(role)}{path}";

            HttpMessage<TReq> message = new()
            {
                Account = "Server",
                Data = req,
            };
            string json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            string resJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<HttpResult<TRes>>(resJson)!;
        }
        private string GetUrlByRole(string role)
        {
            var processes = config
                    .GetSection("Processes")
                    .Get<Dictionary<string, ProcessInfo>>();

            var result = processes?.FirstOrDefault(kvp => kvp.Key == role).Value;
            if (result != null) return $"{result.Host}:{result.Port}";
            return "http://localhost:8080";
        }
    }
}
