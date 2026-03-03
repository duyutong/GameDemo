using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomCreateRequest")]
    public class RoomCreateRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public int PlayerId { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int RoomType { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int AccessType { get; set; }

        #endregion Variable
    }
}
