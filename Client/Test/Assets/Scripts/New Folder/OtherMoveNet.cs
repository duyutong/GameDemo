using Network;
using Network.API;
using Network.Models.Common;
using Network.Transport.Udp;
using Network.Transport.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static EnumDefinitions;

public class OtherMoveNet : MonoBehaviour
{
    public string account = "ABC";
    public float moveLerpSpeed = 30;

    private EOperationState operationState;
    private RectTransform targetRect;
    private Vector2 targetPos;
    private GamePlayApi gamePlayApi => ApiManager.GetWebSoketApi<GamePlayApi>();
    private PlayerMovementApi playerMovementApi => ApiManager.GetUdpApi<PlayerMovementApi>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (targetRect == null) targetRect = transform.GetComponent<RectTransform>();
        playerMovementApi.AddListener<List<MovementInfo>>(NetworkEventPaths.PlayerMovement_MoveInGame, OnRecieveMoveMsg);
        gamePlayApi.AddListener<MovementInfo>(NetworkEventPaths.GamePlay_SetMovementState, OnMovementStateChanged);
    }

    private void OnMovementStateChanged(WebSocketResult<MovementInfo> result)
    {
        if (result.Code != 200) return;
        if (result.Data == null) return;
        if (result.Data.Account != account) return;

        MovementInfo info = result.Data;
        operationState = info.EOpState;
    }

    private void OnRecieveMoveMsg(UdpResult<List<MovementInfo>> result)
    {
        if (result.Code != 200) return;
        if (result.Data == null) return;

        List<MovementInfo> list = result.Data;
        MovementInfo info = list.First((_info) => { return _info.Account == account; });
        
        if (info == null) return;
        if (operationState == EOperationState.Begin)
            operationState = EOperationState.InProgress;
        
        targetPos = new Vector2(info.X, info.Y);
    }
    // Update is called once per frame
    void Update()
    {
        if (operationState != EOperationState.Begin) 
        {
            Vector2 posA = targetRect.anchoredPosition;
            Vector2 posB = targetPos;
            float lerpT = moveLerpSpeed * Time.deltaTime;
            targetRect.anchoredPosition = Vector2.Lerp(posA, targetPos, lerpT);
        }
    }
}
