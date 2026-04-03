using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    [global::ProtoBuf.ProtoContract(Name = @"MapGenerateRequest")]
    public class MapGenerateRequest
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public List<EMapLayerType> MapLayer { get; set; }

        #endregion Variable
    }
}
