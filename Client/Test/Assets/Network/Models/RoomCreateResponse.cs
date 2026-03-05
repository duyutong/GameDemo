using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
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
