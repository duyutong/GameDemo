using System;
using System.Collections.Generic;
using Network.Models.Common;
using System.Numerics;
using static EnumDefinitions;

namespace Network.Models.Common
{
    [global::ProtoBuf.ProtoContract(Name = @"MovementInfo")]
    public class MovementInfo
    {
        #region AutoContext

        [global::ProtoBuf.ProtoMember(1)]
        public string Account { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public float X { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public float Y { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public float Z { get; set; }

        [global::ProtoBuf.ProtoMember(5)]
        public EOperationState EOpState { get; set; }

        [global::ProtoBuf.ProtoMember(6)]
        public float MoveLerpSpeed { get; set; }

        #endregion Variable
    }
}