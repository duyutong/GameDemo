using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
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
