using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network.Models
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
