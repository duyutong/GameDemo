using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"BagAcquireItemRequest")]
    public class BagAcquireItemRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int AreaId { get; set; }

        #endregion Variable
    }
}
