using Network;
using Network.API;
using Network.Models.Common;
using UnityEngine;
using static EnumDefinitions;

public class LocalPlayerMovement : MonoBehaviour
{
    private string account = "ABC";
    private MovementInfo movementInfo;
    private GamePlayApi gamePlayApi => ApiManager.GetWebSoketApi<GamePlayApi>();
    private PlayerMovementApi playerMovementApi => ApiManager.GetUdpApi<PlayerMovementApi>();
    private void Start()
    {
        movementInfo ??= new MovementInfo();
    }
    public void SyncLocalPlayerMovement(EOperationState operation, Vector3 worldPos, float moveLerpSpeed = 30)
    {
        account = NetworkManager.Instance.Account;

        movementInfo.Account = account;
        movementInfo.EOpState = operation;
        movementInfo.MoveLerpSpeed = moveLerpSpeed;
        movementInfo.X = worldPos.x;
        movementInfo.Y = worldPos.y;
        movementInfo.Z = worldPos.z;

        if (operation == EOperationState.Begin || operation == EOperationState.Finish)
            gamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_SetMovementState, movementInfo);
        else if(operation == EOperationState.InProgress)
            playerMovementApi.SendUdpMessage(NetworkEventPaths.PlayerMovement_MoveInGame, movementInfo);
    }
}
