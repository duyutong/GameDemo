using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginHttpLoginRequest")]
    public class LoginHttpLoginRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public string Password { get; set; }

        #endregion Variable
    }
}
