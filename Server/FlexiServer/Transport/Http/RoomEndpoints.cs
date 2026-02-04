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
            
            app.MapPost("/room/create", async (HttpMessage<RoomCreateRequest> msg) =>
            {
                var result = new HttpResult<RoomCreateResponse>();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = service.RoomCreate(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
                }
                catch (ServerException ex)
                {
                    result.Code = 1;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                return result;
            });
            
            app.MapPost("/room/getRooms", async (HttpMessage<RoomGetRoomsRequest> msg) =>
            {
                var result = new HttpResult<RoomGetRoomsResponse>();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = service.RoomGetRooms(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
                }
                catch (ServerException ex)
                {
                    result.Code = 1;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                return result;
            });
            
            app.MapPost("/room/findRoom", async (HttpMessage<RoomFindRoomRequest> msg) =>
            {
                var result = new HttpResult<RoomFindRoomResponse>();
                try
                {
                    RoomService service = app.Services.GetRequiredService<RoomService>();
                    var res = service.RoomFindRoom(msg);
                    result.Code = 200;
                    result.Message = "succ";
                    result.Data = res;
                }
                catch (ServerException ex)
                {
                    result.Code = 1;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                return result;
            });
            
            #endregion MapPostStr
        }
    }
}