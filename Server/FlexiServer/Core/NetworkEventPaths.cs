using System.Collections;
using System.Collections.Generic;
namespace FlexiServer.Core
{
    public class NetworkEventPaths
    {
        #region AutoContext
        public const string Chat_SendMessage = "/sendMessage";
        public const string Chat_NewMessage = "/newMessage";
        public const string GamePlay_JoinGame = "/joinGame";
        public const string GamePlay_StartGame = "/startGame";
        #endregion NetworkEventPaths
    }
}
