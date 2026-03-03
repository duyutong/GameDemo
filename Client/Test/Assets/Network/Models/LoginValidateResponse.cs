using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginValidateResponse")]
    public class LoginValidateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public bool IsValidate { get; set; }

        #endregion Variable
    }
}
