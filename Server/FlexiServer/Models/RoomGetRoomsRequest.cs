using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomGetRoomsRequest")]
    public class RoomGetRoomsRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int RoomType { get; set; }

        #endregion Variable
    }
}
