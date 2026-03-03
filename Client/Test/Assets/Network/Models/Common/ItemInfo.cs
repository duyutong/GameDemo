using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"ItemInfo")]
    public class ItemInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        #endregion Variable
    }
}