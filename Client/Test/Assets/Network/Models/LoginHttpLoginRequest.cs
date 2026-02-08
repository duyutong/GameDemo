using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    public class LoginHttpLoginRequest
    {
        #region AutoContext
        
        public string Account { get; set; }
        public string Password { get; set; }
        #endregion Variable
    }
}
