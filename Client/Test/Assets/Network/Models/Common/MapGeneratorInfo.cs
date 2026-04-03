using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"MapGeneratorInfo")]
    public class MapGeneratorInfo
    {
        #region AutoContext
        
        [global::ProtoBuf.ProtoMember(1)]
        public EMapLayerType MapLayer { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public int Seed { get; set; }

        #endregion Variable
    }
}
