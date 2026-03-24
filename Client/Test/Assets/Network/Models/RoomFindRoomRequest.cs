using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
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
