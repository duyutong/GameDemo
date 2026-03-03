using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace FlexiServer.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"PlayerGameInfo")]
    public class PlayerGameInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        #endregion Variable
    }
}