using FlexiServer.Core;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Web;
namespace FlexiServer.Services
{
    [ProcessFeature("GamePlay")]
    public class GamePlayService(SandboxManager sandboxManager) : IService
    {
        public string Pattern => "/gamePlay";
        public void OnDataRecieved(string ClientId, string Account, byte[] Buffer)
        {
            var recievMsg = Buffer.ConvertData<WebSocketMessage>();
            if (recievMsg == null) return;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[GamePlayService]");
            Console.ResetColor();

            Console.WriteLine(
                $" OnDataRecieved | Pattern: {recievMsg.Pattern} | Path: {recievMsg.Path}"
            );

            switch (recievMsg.Path)
            {
                #region AutoContext

                case NetworkEventPaths.GamePlay_JoinGame:
                    JoinGameHandle(ClientId, Account, recievMsg.Path, Buffer);
                    break;


                case NetworkEventPaths.GamePlay_StartGame:
                    StartGameHandle(ClientId, Account, recievMsg.Path, Buffer);
                    break;


                case NetworkEventPaths.GamePlay_SetMovementState:
                    SetMovementStateHandle(ClientId, Account, recievMsg.Path, Buffer);
                    break;


                case NetworkEventPaths.GamePlay_LeaveGame:
                    LeaveGameHandle(ClientId, Account, recievMsg.Path, Buffer);
                    break;

                 #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext

        private void JoinGameHandle(string clientId, string account, string path, byte[] buffer)
        {
            //测试代码，不筛选沙盒
            sandboxManager.GetOrCreateSandbox<GamePlayItemSandbox>();
            sandboxManager.GetOrCreateSandbox<GamePlayMapSandbox>();
            sandboxManager.GetOrCreateSandbox<GamePlayMovementSandbox>();

            GamePlayItemSandbox? sandbox_item = sandboxManager.GetSandbox<GamePlayItemSandbox>();
            sandbox_item?.AddPlayer(clientId, account);

            GamePlayMapSandbox? sandbox_map = sandboxManager.GetSandbox<GamePlayMapSandbox>();
            sandbox_map?.AddPlayer(clientId, account);

            GamePlayMovementSandbox? sandbox_movement = sandboxManager.GetSandbox<GamePlayMovementSandbox>();
            sandbox_movement?.AddPlayer(clientId, account);
        }

        private void StartGameHandle(string clientId, string account, string path, byte[] buffer)
        {
            var sandbox = sandboxManager.GetOrCreateSandbox<GamePlayMapSandbox>();
            TransportManager.SendMessageToClient<WebSocketTransport, object>(sandbox.GetPlayerClients(), Pattern, path, null);
        }

        private void SetMovementStateHandle(string clientId, string account, string path, byte[] buffer)
        {
            var recievMsg = buffer.ConvertData<WebSocketMessage>();
            if (recievMsg == null) return;
            if (recievMsg.Data == null || recievMsg.Data.Length == 0) return;

            MovementInfo? data = recievMsg.Data.ConvertData<MovementInfo>();
            GamePlayMovementSandbox? sandbox = sandboxManager.GetSandbox<GamePlayMovementSandbox>((_standbox) =>
            { return _standbox.ContainsPlayer(account); });
            if (sandbox == null) return;

            sandbox.RefreshMovementState(data);

            TransportManager.SendMessageToClient<WebSocketTransport, MovementInfo>(sandbox.GetPlayerClients(), Pattern, path, data);
        }

        private void LeaveGameHandle(string clientId, string account, string path, byte[] buffer)
        {

        }
        #endregion Function_Handle
    }
}