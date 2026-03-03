using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Udp;

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
                    var recievMsg = TransportUtil.DeserializeUdpMessage<MovementInfo>(command.Command);
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
            if (sandbox == null) return;

            int ServerCurrentFrame = frameManager.ServerCurrentFrame;
            if (!sandbox.HasSendableUpdate(ServerCurrentFrame)) return;

            var movmentInfos = sandbox.GetMoveInfos(ServerCurrentFrame);
            if (movmentInfos.Count == 0) return;

            string path = NetworkEventPaths.PlayerMovement_MoveInGame;
            TransportManager.SendMessageToClient<UdpTransport, List<MovementInfo>>(sandbox.GetPlayerClients(), Pattern, path, movmentInfos);
            
        }
        public void OnDataRecieved(string ClientId, string Account, byte[] Buffer)
        {
            UdpMessageHeader recievMsg = TransportUtil.DeserializeUdpMessageHeader(Buffer);
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
                    MoveInGameHandle(ClientId, Account, recievMsg.Path, Buffer);
                    break;

                #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext
        private void MoveInGameHandle(string clientId, string account, string path, byte[] buffer)
        {
            var recievMsg = TransportUtil.DeserializeUdpMessage<MovementInfo>(buffer);
            if (recievMsg == null) return;

            frameManager.AddFrameMessageToPool(recievMsg.InputFrame, clientId, Pattern, path, buffer);
        }

        #endregion Function_Handle
    }
}
