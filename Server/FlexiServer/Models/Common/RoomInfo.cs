using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomInfo")]
    public class RoomInfo
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public int RoomId { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int MaxCount { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int CurrCount { get; set; }

        #endregion Variable
    }
}
