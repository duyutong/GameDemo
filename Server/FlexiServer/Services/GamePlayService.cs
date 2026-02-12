using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;
namespace FlexiServer.Services
{
    [ProcessFeature("GamePlay")]
    public class GamePlayService(SandboxManager sandboxManager, FrameManager frameManager) : IService
    {
        public string Pattern => "/gamePlay";
        public void OnDataRecieved(string ClientId, string Account, string Msg)
        {
            WebSocketMessage<object>? recievMsg = JsonConvert.DeserializeObject<WebSocketMessage<object>>(Msg);
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
                    JoinGameHandle(ClientId, Account, recievMsg.Path, Msg);
                    break;

                               
                case NetworkEventPaths.GamePlay_StartGame:
                    StartGameHandle(ClientId, Account, recievMsg.Path, Msg);
                    break;

                               
                case NetworkEventPaths.GamePlay_SetMovementState:
                    SetMovementStateHandle(ClientId, Account, recievMsg.Path, Msg);
                    break;

                               
                case NetworkEventPaths.GamePlay_LeaveGame:
                    LeaveGameHandle(ClientId, Account, recievMsg.Path, Msg);
                    break;

                 #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext
        
        private void JoinGameHandle(string clientId, string account, string path, string msg)
        {
            //���Դ��룬��ɸѡɳ��
            GamePlayItemSandbox? sandbox_item = sandboxManager.GetSandbox<GamePlayItemSandbox>();
            sandbox_item?.AddPlayer(clientId, account);

            GamePlayMovementSandbox? sandbox_movement = sandboxManager.GetSandbox<GamePlayMovementSandbox>();
            sandbox_movement?.AddPlayer(clientId, account);
        }
        
        private void StartGameHandle(string clientId, string account, string path, string msg)
        {
            sandboxManager.GetOrCreateSandbox<GamePlayItemSandbox>();
            sandboxManager.GetOrCreateSandbox<GamePlayMovementSandbox>();
        }
        
        private void SetMovementStateHandle(string clientId, string account, string path, string msg)
        {
            var recievMsg = JsonConvert.DeserializeObject<WebSocketMessage<MovementInfo>>(msg);
            MovementInfo? info = recievMsg!.Data;

            GamePlayMovementSandbox? sandbox = sandboxManager.GetSandbox<GamePlayMovementSandbox>((_standbox) =>
            { return _standbox.ContainsPlayer(account); });
            if (sandbox == null) return;

            sandbox.RefreshMovementState(info);

            WebSocketResult<MovementInfo> sendMsg = new WebSocketResult<MovementInfo>();
            sendMsg.Pattern = Pattern;
            sendMsg.Path = path;
            sendMsg.Data = info;
            sendMsg.Type = EWsMessageType.FrameSync;
            sendMsg.ServerFrame = frameManager.ServerCurrentFrame;
            sendMsg.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string wsMsgStr = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<WebSocketTransport>(sandbox.GetPlayerClients(), wsMsgStr);
        }
        
        private void LeaveGameHandle(string clientId, string account, string path, string msg)
        {
            
        }
        #endregion Function_Handle
    }
}