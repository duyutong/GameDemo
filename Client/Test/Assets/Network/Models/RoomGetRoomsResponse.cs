using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomGetRoomsResponse")]
    public class RoomGetRoomsResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public List<RoomInfo> Rooms { get; set; }

        #endregion Variable
    }
}
