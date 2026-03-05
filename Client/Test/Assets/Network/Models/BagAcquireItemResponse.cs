using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"BagAcquireItemResponse")]
    public class BagAcquireItemResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public ItemInfo Item { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int BagSlotIndex { get; set; }

        #endregion Variable
    }
}
