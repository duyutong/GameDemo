using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomFindRoomResponse")]
    public class RoomFindRoomResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public RoomInfo RoomInfo { get; set; }

        #endregion Variable
    }
}
