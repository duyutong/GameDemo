using System;
using System.Collections.Generic;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models.Common
{
    public class RoomInfo
    {
        #region AutoContext
        
        public int RoomId { get; set; }
        public int MaxCount { get; set; }
        public int CurrCount { get; set; }
        #endregion Variable
    }
}
