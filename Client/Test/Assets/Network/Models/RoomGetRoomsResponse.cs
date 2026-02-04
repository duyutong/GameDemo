using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models
{
    public class RoomGetRoomsResponse
    {
        #region AutoContext
        
        public List<RoomInfo> Rooms { get; set; }
        #endregion Variable
    }
}
