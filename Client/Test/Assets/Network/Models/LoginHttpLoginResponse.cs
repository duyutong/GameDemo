using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginHttpLoginResponse")]
    public class LoginHttpLoginResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Token { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public List<ProcessInfo> ProcessInfos { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public int Code { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public int FrameSyncIntervalMs { get; set; }

        #endregion Variable
    }
}
