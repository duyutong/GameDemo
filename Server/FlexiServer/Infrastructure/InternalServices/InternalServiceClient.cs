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
        public async Task<HttpResult<TRes>> PostAsync<TReq, TRes>(string role, string path, TReq req)
        {
            string url = $"{GetUrlByRole(role)}{path}";

            HttpMessage<TReq> message = new()
            {
                Account = "Server",
                Data = req,
            };
            HttpContent content;
            byte[] bytes;
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, message);
                bytes = ms.ToArray();
            }
            content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-protobuf");

            var response = await client.PostAsync(url, content);
            byte[] resBytes = await response.Content.ReadAsByteArrayAsync();

            return TransportUtil.DeserializeHttpResult<TRes>(resBytes);
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
