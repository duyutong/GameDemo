using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models.Common
{
    public class MovementInfo
    {
        #region AutoContext
        
        public string Account { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public EOperationState EOpState { get; set; }
        public float MoveLerpSpeed { get; set; }
        #endregion Variable
    }
}
