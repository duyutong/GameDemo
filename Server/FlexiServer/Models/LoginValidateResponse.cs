using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginValidateResponse")]
    public class LoginValidateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public bool IsValidate { get; set; }

        #endregion Variable
    }
}
