using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace FlexiServer.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"ClueInfo")]
    public class ClueInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public int Id { get; set; }

        #endregion Variable
    }
}