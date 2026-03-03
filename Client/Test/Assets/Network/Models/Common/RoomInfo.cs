using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomInfo")]
    public class RoomInfo
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int RoomId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int MaxCount { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int CurrCount { get; set; }

        #endregion Variable
    }
}
