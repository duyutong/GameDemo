namespace FlexiServer.Transport.Http
{
    public partial class MapPostEndpoints
    {
        public static async void ReturnHttpResultTask(HttpContext context, HttpResult result)
        {
            byte[] resBytes = result.ToBytes();
            context.Response.ContentLength = resBytes.Length;
            await context.Response.Body.WriteAsync(resBytes, 0, resBytes.Length);
        }
        public static async Task<HttpMessage> ReadHttpMessageAsync(HttpContext context)
        {
            using var ms = new MemoryStream();
            await context.Request.Body.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            return bytes.ConvertData<HttpMessage>()!;
        }
    }
}
