using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomGetRoomsRequest")]
    public class RoomGetRoomsRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public int RoomType { get; set; }

        #endregion Variable
    }
}
