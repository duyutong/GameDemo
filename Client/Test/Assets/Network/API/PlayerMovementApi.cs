using Network.Models.Common;
using Network.Transport.Udp;
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
        public override void OnDataRecieved(string pattern, UdpResult<object> result)
        {
            base.OnDataRecieved(pattern, result);
        }
    }
}
