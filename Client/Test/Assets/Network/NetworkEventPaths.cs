using System.Collections;
using System.Collections.Generic;
namespace Network
{
    public class NetworkEventPaths
    {
        #region AutoContext
        public const string GamePlay_JoinGame = "/joinGame";
        public const string GamePlay_StartGame = "/startGame";
        public const string PlayerMovement_MoveInGame = "/moveInGame";
        public const string GamePlay_SetMovementState = "/setMovementState";
        public const string GamePlay_LeaveGame = "/leaveGame";
        #endregion NetworkEventPaths
    }
}
