using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    public class RoomCreateRequest
    {
        #region AutoContext
        
        public int PlayerId { get; set; }
        public int RoomType { get; set; }
        public int AccessType { get; set; }
        #endregion Variable
    }
}
