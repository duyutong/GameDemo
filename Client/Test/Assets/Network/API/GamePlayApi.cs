using Network.Transport.WebSocket;
using System;
namespace Network.API
{
    public class GamePlayApi : WebSocketMessageApi
    {
        public override string Pattern { get; set; } = "/gamePlay";

        public void SendWebSocketMessage<TSend>(string path, TSend messageObj)
        {
            base.SendWebSocketMessage(Pattern, path, messageObj);
        }
        public override void OnDataRecieved(string pattern, WebSocketResult result)
        {
            base.OnDataRecieved(pattern, result);
        }
    }
}