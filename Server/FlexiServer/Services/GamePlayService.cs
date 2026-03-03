using FlexiServer.Core;
using FlexiServer.Core.Frame;
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
            WebSocketMessageHeader recievMsg = TransportUtil.DeserializeWsMessageHeader(Buffer);
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
            GamePlayItemSandbox? sandbox_item = sandboxManager.GetSandbox<GamePlayItemSandbox>();
            sandbox_item?.AddPlayer(clientId, account);

            GamePlayMovementSandbox? sandbox_movement = sandboxManager.GetSandbox<GamePlayMovementSandbox>();
            sandbox_movement?.AddPlayer(clientId, account);
        }

        private void StartGameHandle(string clientId, string account, string path, byte[] buffer)
        {
            sandboxManager.GetOrCreateSandbox<GamePlayItemSandbox>();
            sandboxManager.GetOrCreateSandbox<GamePlayMovementSandbox>();
        }

        private void SetMovementStateHandle(string clientId, string account, string path, byte[] buffer)
        {
            var recievMsg = TransportUtil.DeserializeWsMessage<MovementInfo>(buffer);
            MovementInfo? data = recievMsg!.Data;

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