using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
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
