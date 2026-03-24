using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomFindRoomRequest")]
    public class RoomFindRoomRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int RoomId { get; set; }

        #endregion Variable
    }
}
