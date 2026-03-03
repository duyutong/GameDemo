using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueIdentifyRequest")]
    public class ClueIdentifyRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public int BagIndex { get; set; }

        #endregion Variable
    }
}
