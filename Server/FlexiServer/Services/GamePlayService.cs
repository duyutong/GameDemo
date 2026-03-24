using FlexiServer.Core;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;
using System.Collections.Concurrent;
namespace FlexiServer.Services
{
    [ProcessFeature("GamePlay")]
    public class GamePlayService(SandboxManager sandboxManager) : IService
    {
        public string Pattern => "/gamePlay";
        public void OnDataRecieved(string ClientId, string Acount, string Msg)
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
                    JoinGameHandle(ClientId, Acount, recievMsg.Path, Msg);
                    break;
                case NetworkEventPaths.GamePlay_StartGame:
                    StartGameHandle(ClientId, recievMsg.Path, Msg);
                    break;
                 #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext

        private void JoinGameHandle(string ClientId, string Acount, string Path, string Msg)
        {
            GamePlayItemSandbox? sandbox = sandboxManager.GetSandBox<GamePlayItemSandbox>((_standbox) => true);
            sandbox?.AddPlayer(ClientId, Acount);

            WebSocketMessage<string> sendMsg = new WebSocketMessage<string>();
            sendMsg.Pattern = Pattern;
            sendMsg.Path = Path;
            sendMsg.Data = "Succ";
            sendMsg.Type = EWsMessageType.Normal;
            string wsMsgStr = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<WebSocketTransport>(ClientId, wsMsgStr);
        }

        private void StartGameHandle(string ClientId, string Path, string Msg)
        {
            sandboxManager.GetOrCreateSandBox<GamePlayItemSandbox>();

            WebSocketMessage<string> sendMsg = new WebSocketMessage<string>();
            sendMsg.Pattern = Pattern;
            sendMsg.Path= Path;
            sendMsg.Data = "Succ";
            sendMsg.Type = EWsMessageType.Normal;
            string wsMsgStr = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<WebSocketTransport>(ClientId, wsMsgStr);
        }
        
        private void JoinGameHandle(string ClientId, string Path, string Msg)
        {
            
        }
        #endregion Function_Handle
    }
}