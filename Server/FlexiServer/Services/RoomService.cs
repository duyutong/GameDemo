using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Room")]
    public class RoomService
    {
        #region AutoContext
        public async Task<RoomCreateResponse> RoomCreate(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, "Data is Null");

            RoomCreateRequest? req = msg.Data.ConvertData<RoomCreateRequest>();
            if (req == null) throw new ServerException(ErrorCode.None, "RoomCreateRequest is Null");
            
            RoomCreateResponse res = new();
            return res;
        }
        
        public async Task<RoomGetRoomsResponse> RoomGetRooms(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, "Data is Null");

            RoomGetRoomsRequest? req = msg.Data.ConvertData<RoomGetRoomsRequest>();
            if (req == null) throw new ServerException(ErrorCode.None, "RoomGetRoomsRequest is Null");
            
            RoomGetRoomsResponse res = new();
            return res;
        }
        
        public async Task<RoomFindRoomResponse> RoomFindRoom(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, "Data is Null");

            RoomFindRoomRequest? req = msg.Data.ConvertData<RoomFindRoomRequest>();
            if (req == null) throw new ServerException(ErrorCode.None, "RoomFindRoomRequest is Null");
            
            RoomFindRoomResponse res = new();
            return res;
        }
        #endregion HttpFuncStr
    }
}