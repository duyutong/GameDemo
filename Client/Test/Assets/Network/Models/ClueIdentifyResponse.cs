using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueIdentifyResponse")]
    public class ClueIdentifyResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public ClueInfo ClueInfo { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int BagIndex { get; set; }

        #endregion Variable
    }
}
