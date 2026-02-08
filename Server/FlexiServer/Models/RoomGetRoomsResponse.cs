using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    public class RoomGetRoomsResponse
    {
        #region AutoContext
        
        public List<RoomInfo> Rooms { get; set; }
        #endregion Variable
    }
}
