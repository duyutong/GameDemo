using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"MapGenerateResponse")]
    public class MapGenerateResponse
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public List<MapGeneratorInfo> MapGenInfos { get; set; }

        #endregion Variable
    }
}
