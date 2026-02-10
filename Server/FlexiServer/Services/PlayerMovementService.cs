using FlexiServer.Core;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Udp;
using FlexiServer.Transport.Web;
using Newtonsoft.Json;

namespace FlexiServer.Services
{
    [ProcessFeature("PlayerMovement")]
    public class PlayerMovementService : IService
    {
        public string Pattern => "/playerMovement";

        public void OnDataRecieved(string ClientId, string Acount, string Msg)
        {
            TransportMessage? recievMsg = JsonConvert.DeserializeObject<TransportMessage>(Msg);
            if (recievMsg == null) return;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[PlayerMovementService]");
            Console.ResetColor();

            Console.WriteLine(
                $" OnDataRecieved | Pattern: {recievMsg.Pattern} | Path: {recievMsg.Path}"
            );

            switch (recievMsg.Path)
            {
                #region AutoContext

                case NetworkEventPaths.PlayerMovement_MoveInGame:
                    MoveInGameHandle(ClientId, Acount, recievMsg.Path, Msg);
                    break;

                #endregion Switch_Handle
                default:
                    break;
            }
        }

        private void MoveInGameHandle(string clientId, string acount, string path, string msg)
        {
            UdpMessage<string> sendMsg = new UdpMessage<string>();
            sendMsg.Pattern = Pattern;
            sendMsg.Path = path;
            sendMsg.Data = "Succ";
            string udpMsgStr = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<UdpTransport>(acount, udpMsgStr);
        }
    }
}
