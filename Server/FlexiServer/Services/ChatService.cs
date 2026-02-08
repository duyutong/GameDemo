using FlexiServer.Core;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Sandbox.Interface;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;

namespace FlexiServer.Services
{
    [ProcessFeature("Chat")]
    public class ChatService : IService
    {
        public string Pattern => "/chat";
        public void OnDataRecieved(string ClientId, string Acount, string Msg)
        {
            WebSocketMessage<object>? recievMsg = JsonConvert.DeserializeObject<WebSocketMessage<object>>(Msg);
            if (recievMsg == null) return;
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[ChatService]");
            Console.ResetColor();

            Console.WriteLine(
                $" OnDataRecieved | Pattern: {recievMsg.Pattern} | Path: {recievMsg.Path}"
            );


            switch (recievMsg.Path)
            {
                #region AutoContext
                case NetworkEventPaths.Chat_SendMessage:
                    SendMessageHandle(ClientId, recievMsg.Path, Msg);
                    break;
                case NetworkEventPaths.Chat_NewMessage:
                    NewMessageHandle(ClientId, recievMsg.Path, Msg);
                    break;
                #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext
        private void SendMessageHandle(string ClientId, string Path, string Msg)
        {
            var chatMsg = JsonConvert.DeserializeObject<WebSocketMessage<ChatInfo>>(Msg);
            if (chatMsg == null || chatMsg.Data == null) return;

            ChatInfo chatInfo = chatMsg.Data;
            WebSocketResult<ChatInfo> sendMsg = new WebSocketResult<ChatInfo>();
            sendMsg.Pattern = Pattern;
            sendMsg.Path = Path;
            sendMsg.Data = chatInfo;
            sendMsg.Type = EWsMessageType.Normal;
            string wsMsgStr = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<WebSocketTransport>(ClientId, wsMsgStr);
        }

        private void NewMessageHandle(string ClientId, string Path, string Msg)
        {
            
        }
        #endregion Function_Handle
    }
}