using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Room")]
    public class RoomService
    {
        #region AutoContext
        public RoomCreateResponse RoomCreate(HttpMessage<RoomCreateRequest> msg)
        {
            RoomCreateRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "RoomCreateRequest is Null");

            RoomCreateResponse res = new RoomCreateResponse();
            return res;
        }
        public RoomGetRoomsResponse RoomGetRooms(HttpMessage<RoomGetRoomsRequest> msg)
        {
            RoomGetRoomsRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "RoomGetRoomsRequest is Null");

            RoomGetRoomsResponse res = new RoomGetRoomsResponse();
            res.Rooms = new List<RoomInfo>();
            return res;
        }

        public RoomFindRoomResponse RoomFindRoom(HttpMessage<RoomFindRoomRequest> msg)
        {
            RoomFindRoomRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "RoomFindRoomRequest is Null");

            RoomFindRoomResponse res = new RoomFindRoomResponse();
            return res;
        }
        #endregion HttpFuncStr
    }
}