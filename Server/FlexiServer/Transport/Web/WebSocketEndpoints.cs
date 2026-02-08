namespace FlexiServer.Transport.Web
{
    public static class WebSocketEndpoints
    {

        public static void MapWebSocketEndpoints(this WebApplication app)
        {
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocketTransport transport = app.Services.GetRequiredService<WebSocketTransport>();
                    var token = context.Request.Query["token"].ToString();
                    var ws = await context.WebSockets.AcceptWebSocketAsync();
                    Console.WriteLine("客户端已连接 WebSocketManager");

                    TokenManager tokenService = app.Services.GetRequiredService<TokenManager>();
                    bool isValidate = await tokenService.Validate(token);
                    if (isValidate) await transport.AddClient(token, ws); 
                    else await transport.RequestReLoginAsync(ws);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });

            // 传统 HTTP 路由
            app.MapGet("/ping", () => "pong");
        }
    }
}
