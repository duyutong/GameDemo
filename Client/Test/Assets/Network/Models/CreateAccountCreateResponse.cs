using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"CreateAccountCreateResponse")]
    public class CreateAccountCreateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Token { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public List<ProcessInfo> ProcessInfos { get; set; }

        #endregion Variable
    }
}
