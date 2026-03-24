using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueIdentifyRequest")]
    public class ClueIdentifyRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public int BagIndex { get; set; }

        #endregion Variable
    }
}
