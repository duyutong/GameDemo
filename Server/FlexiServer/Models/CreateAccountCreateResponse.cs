using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"CreateAccountCreateResponse")]
    public class CreateAccountCreateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public string Token { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public List<ProcessInfo> ProcessInfos { get; set; }

        #endregion Variable
    }
}
