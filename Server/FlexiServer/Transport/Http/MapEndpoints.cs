using FlexiServer.Core;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("Map")]
        public static void MapMapEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/map/generate", async (HttpContext context) =>
            {
                HttpMessage msg = await ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    MapService service = app.Services.GetRequiredService<MapService>();
                    var res = await service.MapGenerate(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res.ToBytes();
                }
                catch (ServerException ex)
                {
                    result.Code = ex.Code;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                ReturnHttpResultTask(context, result);
            });
            
            #endregion MapPostStr
        }
    }
}