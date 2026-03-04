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
        public override void AddListener(string path, Action<WebSocketResult> callBack)
        {
            base.AddListener(path, callBack);
        }
        public override void RemoveListener(string path, Action<WebSocketResult> callBack)
        {
            base.RemoveListener(path, callBack);
        }
    }
}