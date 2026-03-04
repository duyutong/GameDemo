using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("Clue")]
        public static void MapClueEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/clue/identify", async (HttpContext context) =>
            {
                HttpMessage msg = await TransportUtil.ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    ClueService service = app.Services.GetRequiredService<ClueService>();
                    var res = await service.ClueIdentify(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res.ToBytes();
                }
                catch (ServerException ex)
                {
                    result.Code = ex.Code;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                TransportUtil.ReturnHttpResultTask(context, result);
            });
            
            #endregion MapPostStr
        }
    }
}