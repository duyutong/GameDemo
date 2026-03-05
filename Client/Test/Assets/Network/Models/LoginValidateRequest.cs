using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"LoginValidateRequest")]
    public class LoginValidateRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        #endregion Variable
    }
}
