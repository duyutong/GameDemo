using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    public class BagAcquireItemResponse
    {
        #region AutoContext
        
        public ItemInfo Item { get; set; }
        public int BagSlotIndex { get; set; }
        #endregion Variable
    }
}
