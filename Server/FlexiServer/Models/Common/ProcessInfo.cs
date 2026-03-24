using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace FlexiServer.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"ProcessInfo")]
    public class ProcessInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public string Role { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Host { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public int Port { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public List<string> Modules { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public bool UseWebSocket { get; set; }

        #endregion Variable
    }
}