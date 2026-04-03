using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
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
