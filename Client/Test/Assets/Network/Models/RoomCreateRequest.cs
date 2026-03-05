using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"RoomCreateRequest")]
    public class RoomCreateRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int PlayerId { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int RoomType { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int AccessType { get; set; }

        #endregion Variable
    }
}
