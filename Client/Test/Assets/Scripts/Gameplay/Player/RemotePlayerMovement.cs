using Network;
using Network.API;
using Network.Models.Common;
using Network.Transport.Udp;
using Network.Transport.WebSocket;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EnumDefinitions;

public class RemotePlayerMovement : MonoBehaviour
{
    public string account = "ABC";
    public bool freezeZ = true; // �Ƿ񶳽� Z ��

    private int lastFramerate = 0;
    private float moveLerpSpeed = 30;
    private EOperationState operationState;
    private Vector3 targetPos;
    private GamePlayApi gamePlayApi => ApiManager.GetWebSoketApi<GamePlayApi>();
    private PlayerMovementApi playerMovementApi => ApiManager.GetUdpApi<PlayerMovementApi>();

    void Start()
    {
        playerMovementApi.AddListener<List<MovementInfo>>(NetworkEventPaths.PlayerMovement_MoveInGame, OnRecieveMoveMsg);
        gamePlayApi.AddListener<MovementInfo>(NetworkEventPaths.GamePlay_SetMovementState, OnMovementStateChanged);
    }
    private void OnDestroy()
    {
        playerMovementApi.RemoveListener<List<MovementInfo>>(NetworkEventPaths.PlayerMovement_MoveInGame, OnRecieveMoveMsg);
        gamePlayApi.RemoveListener<MovementInfo>(NetworkEventPaths.GamePlay_SetMovementState, OnMovementStateChanged);
    }
    private void OnMovementStateChanged(WebSocketResult<MovementInfo> result)
    {
        if (result.Code != 200) return;
        if (result.Data == null) return;
        if (result.Data.Account != account) return;
        if (result.ServerFrame < lastFramerate) return;

        lastFramerate = Mathf.Max(lastFramerate, result.ServerFrame);
        MovementInfo info = result.Data;
        moveLerpSpeed = info.MoveLerpSpeed;
        operationState = info.EOpState;
        targetPos = new Vector3(info.X, info.Y, freezeZ ? transform.position.z : info.Z);
    }

    private void OnRecieveMoveMsg(UdpResult<List<MovementInfo>> result)
    {
        if (result.Code != 200) return;
        if (result.Data == null) return;
        if (result.ServerFrame < lastFramerate) return;

        List<MovementInfo> list = result.Data;
        MovementInfo info = list.First((_info) => { return _info.Account == account; });

        if (info == null) return;

        operationState = info.EOpState;
        moveLerpSpeed = info.MoveLerpSpeed;
        targetPos = new Vector3(info.X, info.Y, freezeZ ? transform.position.z : info.Z);
        lastFramerate = Mathf.Max(lastFramerate, result.ServerFrame);
    }
    void FixedUpdate()
    {
        if (operationState != EOperationState.None)
        {
            Vector3 posA = transform.position;
            float lerpT = moveLerpSpeed * Time.fixedDeltaTime;
            transform.position = Vector3.Lerp(posA, targetPos, lerpT);
        }
    }
}
