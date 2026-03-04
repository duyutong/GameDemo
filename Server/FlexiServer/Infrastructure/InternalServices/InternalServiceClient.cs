using FlexiServer.Models.Common;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
using Newtonsoft.Json;
using System.Text;

namespace FlexiServer.Infrastructure.InternalServices
{
    public class InternalServiceClient(IConfiguration config)
    {
        private HttpClient client = new HttpClient();
        public async Task<HttpResult> PostAsync<TReq, TRes>(string role, string path, TReq req)
        {
            string url = $"{GetUrlByRole(role)}{path}";

            HttpMessage message = new()
            {
                Account = "Server",
                Data = req.ToBytes(),
            };

            HttpContent content = GetHttpContent(message);
            var response = await client.PostAsync(url, content);
            byte[] resBytes = await response.Content.ReadAsByteArrayAsync();

            return resBytes.ConvertData<HttpResult>()!;
        }
        private HttpContent GetHttpContent(HttpMessage message)
        {
            HttpContent content;
            byte[] bytes = message.ToBytes();
            content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");

            return content;
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
