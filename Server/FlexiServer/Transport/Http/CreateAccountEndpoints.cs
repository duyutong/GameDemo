using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("CreateAccount")]
        public static void MapCreateAccountEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/createAccount/create", async (HttpMessage<CreateAccountCreateRequest> msg) =>
            {
                var result = new HttpResult<CreateAccountCreateResponse>();
                try
                {
                    CreateAccountService service = app.Services.GetRequiredService<CreateAccountService>();
                    var res = await service.CreateAccountCreate(msg);
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