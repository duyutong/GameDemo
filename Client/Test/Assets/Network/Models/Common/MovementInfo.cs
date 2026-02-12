using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace Network.Models.Common
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
