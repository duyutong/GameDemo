using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models.Common
{
    public class ProcessInfo
    {
        #region AutoContext
        
        public string Role { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<string> Modules { get; set; }
        public bool UseWebSocket { get; set; }
        #endregion Variable
    }
}
