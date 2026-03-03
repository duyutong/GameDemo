using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("Login")]
        public static void MapLoginEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/login/httpLogin", async (HttpContext context) =>
            {
                HttpMessage<LoginHttpLoginRequest> msg = await TransportUtil.ReadHttpMessageAsync<LoginHttpLoginRequest>(context);
                var result = new HttpResult<LoginHttpLoginResponse>();
                try
                {
                    LoginService service = app.Services.GetRequiredService<LoginService>();
                    var res = await service.LoginHttpLogin(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
                }
                catch (ServerException ex)
                {
                    result.Code = ex.Code;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                TransportUtil.ReturnHttpResultTask(context, result);
            });
            
            app.MapPost("/login/Validate", async (HttpContext context) =>
            {
                HttpMessage<LoginValidateRequest> msg = await TransportUtil.ReadHttpMessageAsync<LoginValidateRequest>(context);
                var result = new HttpResult<LoginValidateResponse>();
                try
                {
                    LoginService service = app.Services.GetRequiredService<LoginService>();
                    var res = await service.LoginValidate(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
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