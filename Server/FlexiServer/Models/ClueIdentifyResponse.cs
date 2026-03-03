using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueIdentifyResponse")]
    public class ClueIdentifyResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(0)]
        public ClueInfo ClueInfo { get; set; }

        [global::ProtoBuf.ProtoMember(0)]
        public int BagIndex { get; set; }

        #endregion Variable
    }
}
