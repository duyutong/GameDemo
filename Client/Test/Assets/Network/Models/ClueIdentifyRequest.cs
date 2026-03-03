using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueIdentifyRequest")]
    public class ClueIdentifyRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int BagIndex { get; set; }

        #endregion Variable
    }
}
