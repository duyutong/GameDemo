using UnityEngine;

namespace Network.API
{
    public class PlayerMovementApi : UdpMessageApi
    {
        public override string Pattern { get; set; } = "/playerMovement";
        public void SendUdpMessage<TSend>(string path, TSend messageObj)
        {
            SendUdpMessage(Pattern, path, messageObj);
        }
        public override void OnDataRecieved(string pattern, string msg)
        {
            Debug.Log($"[PlayerMovementApi] OnDataRecieved {msg}");
            base.OnDataRecieved(pattern, msg);
        }
    }
}
