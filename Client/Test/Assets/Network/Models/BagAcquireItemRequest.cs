using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"BagAcquireItemRequest")]
    public class BagAcquireItemRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int AreaId { get; set; }

        #endregion Variable
    }
}
