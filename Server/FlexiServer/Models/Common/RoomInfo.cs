using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models.Common
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
