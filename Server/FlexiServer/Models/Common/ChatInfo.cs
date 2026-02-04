using FlexiServer.Infrastructure.Database;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models.Common
{
    public class ChatInfo
    {
        #region AutoContext
        
        public int SendPlayer { get; set; }
        public int ReceivedPlayer { get; set; }
        public EChatMsgType MsgType { get; set; }
        public string Content { get; set; }
        #endregion Variable
    }
}
