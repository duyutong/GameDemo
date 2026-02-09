using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    public class ClueIdentifyResponse
    {
        #region AutoContext
        
        public ClueInfo ClueInfo { get; set; }
        public int BagIndex { get; set; }
        #endregion Variable
    }
}
