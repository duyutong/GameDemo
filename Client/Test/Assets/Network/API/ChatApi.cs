using Network;
using Network.Models.Common;
using Network.Transport.WebSocket;
using Newtonsoft.Json;
using System;
using UnityEngine;
namespace Network.API
{
    public class ChatApi : WebSocketMessageApi
    {
        public override string Pattern { get; set; } = "/chat";

        public void SendWebSocketMessage<TSend>(string path, TSend messageObj)
        {
            base.SendWebSocketMessage(Pattern, path, messageObj);
        }
        public override void AddListener<TResult>(string path, Action<WebSocketResult<TResult>> callBack)
        {
            base.AddListener(path, callBack);
        }
        public override void RemoveListener<TResult>(string path, Action<WebSocketResult<TResult>> callBack)
        {
            base.RemoveListener(path, callBack);
        }
        public override void OnDataRecieved(string pattern, string msg)
        {
            Debug.Log($"[ChatApi] OnDataRecieved {msg}");
            base.OnDataRecieved(pattern, msg);
        }
    }
}