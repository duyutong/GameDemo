using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("Bag")]
        public static void MapBagEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/bag/acquireItem", async (HttpMessage<BagAcquireItemRequest> msg) =>
            {
                var result = new HttpResult<BagAcquireItemResponse>();
                try
                {
                    BagService service = app.Services.GetRequiredService<BagService>();
                    var res = await service.BagAcquireItem(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
                }
                catch (ServerException ex)
                {
                    result.Code = ex.Code;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                return result;
            });
            
            #endregion MapPostStr
        }
    }
}