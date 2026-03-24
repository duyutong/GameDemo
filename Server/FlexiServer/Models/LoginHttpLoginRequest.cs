using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginHttpLoginRequest")]
    public class LoginHttpLoginRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public string Password { get; set; }

        #endregion Variable
    }
}
