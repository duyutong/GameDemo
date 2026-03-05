using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomCreateResponse")]
    public class RoomCreateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public RoomInfo RoomInfo { get; set; }

        #endregion Variable
    }
}
