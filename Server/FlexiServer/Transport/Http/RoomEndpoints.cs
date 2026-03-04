using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature("Room")]
        public static void MapRoomEndpoints(this WebApplication app)
        {
            #region AutoContext
            
            app.MapPost("/room/create", async (HttpContext context) =>
            {
                HttpMessage msg = await TransportUtil.ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = await service.RoomCreate(msg);
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
            
            app.MapPost("/room/getRooms", async (HttpContext context) =>
            {
                HttpMessage msg = await TransportUtil.ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = await service.RoomGetRooms(msg);
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
            
            app.MapPost("/room/findRoom", async (HttpContext context) =>
            {
                HttpMessage msg = await TransportUtil.ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = await service.RoomFindRoom(msg);
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