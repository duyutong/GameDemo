using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"BagAcquireItemResponse")]
    public class BagAcquireItemResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public ItemInfo Item { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int BagSlotIndex { get; set; }

        #endregion Variable
    }
}
