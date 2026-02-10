using Network;
using Network.API;
using Network.Models.Common;
using UnityEngine;

public class UdpTest : MonoBehaviour
{
    PlayerMovementApi playerMovementApi => ApiManager.GetUdpApi<PlayerMovementApi>();
    public void OnClick() 
    {
        playerMovementApi.SendUdpMessage<string>(NetworkEventPaths.PlayerMovement_MoveInGame, "PlayerMovement");
    }
}
