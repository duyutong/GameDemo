using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Udp;
using Newtonsoft.Json;

namespace FlexiServer.Services
{
    [ProcessFeature("PlayerMovement")]
    public class PlayerMovementService(SandboxManager sandboxManager, FrameManager frameManager) : IService, IFrameResolvedHandler, ISandboxUpdateHandler<GamePlayMovementSandbox>
    {
        public string Pattern => "/playerMovement";

        public void OnFrameResolved(int frame, List<FrameMessage> commands)
        {
            List<MovementInfo> movementInfos = [];
            foreach (var command in commands)
            {
                if (command.Path == NetworkEventPaths.PlayerMovement_MoveInGame)
                {
                    var recievMsg = JsonConvert.DeserializeObject<UdpMessage<MovementInfo>>(command.Command);
                    if (recievMsg == null) continue;
                    if (recievMsg.Data == null) continue;
                    movementInfos.Add(recievMsg.Data);
                }
            }

            foreach (var command in movementInfos)
            {
                var sandbox = sandboxManager.GetSandbox<GamePlayMovementSandbox>((_sandbox) => { return _sandbox.ContainsPlayer(command.Account); });
                if (sandbox == null) continue;
                
                sandbox.RefreshMovement(frame, command);
            }
        }
        public void OnSandboxUpdate(GamePlayMovementSandbox sandbox)
        {
            int ServerCurrentFrame = frameManager.ServerCurrentFrame;
            if (!sandbox.HasSendableUpdate(ServerCurrentFrame)) return;

            var movmentInfos = sandbox.GetMoveInfos(ServerCurrentFrame);
            if (movmentInfos.Count == 0) return;
            
            UdpResult<List<MovementInfo>> sendMsg = new();
            sendMsg.Pattern = Pattern;
            sendMsg.Path = NetworkEventPaths.PlayerMovement_MoveInGame;
            sendMsg.Data = movmentInfos;
            sendMsg.ServerFrame = ServerCurrentFrame;
            sendMsg.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            string udpMsgJson = JsonConvert.SerializeObject(sendMsg);
            TransportManager.SendMessageToClient<UdpTransport>(sandbox.GetPlayerAccounts(), udpMsgJson);
        }
        public void OnDataRecieved(string ClientId, string Account, string Msg)
        {
            TransportMessage? recievMsg = JsonConvert.DeserializeObject<TransportMessage>(Msg);
            if (recievMsg == null) return;

            //Console.ForegroundColor = ConsoleColor.White;
            //Console.Write("[PlayerMovementService]");
            //Console.ResetColor();

            //Console.WriteLine(
            //    $" OnDataRecieved | Pattern: {recievMsg.Pattern} | Path: {recievMsg.Path}"
            //);

            switch (recievMsg.Path)
            {
                #region AutoContext

                case NetworkEventPaths.PlayerMovement_MoveInGame:
                    MoveInGameHandle(ClientId, Account, recievMsg.Path, Msg);
                    break;

                #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext
        private void MoveInGameHandle(string clientId, string account, string path, string msg)
        {
            var recievMsg = JsonConvert.DeserializeObject<UdpMessage<MovementInfo>>(msg);
            if (recievMsg == null) return;

            frameManager.AddFrameMessageToPool(recievMsg.InputFrame, clientId, Pattern, path, msg);
        }

        #endregion Function_Handle
    }
}
