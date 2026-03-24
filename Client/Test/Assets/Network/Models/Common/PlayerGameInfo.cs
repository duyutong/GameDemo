using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"PlayerGameInfo")]
    public class PlayerGameInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        #endregion Variable
    }
}