using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"AccountInfo")]
    public class AccountInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Password { get; set; }

        #endregion Variable
    }
}