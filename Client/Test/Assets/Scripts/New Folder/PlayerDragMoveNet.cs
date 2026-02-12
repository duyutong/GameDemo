using Network;
using Network.API;
using Network.Models;
using Network.Models.Common;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static EnumDefinitions;

public class PlayerDragMoveNet : MonoBehaviour
{
    public string account = "ABC";
    public float moveLerpSpeed = 30;

    private EventTrigger trigger;
    private RectTransform targetRect;
    private RectTransform parentRect;
    private Vector2 dragOffset;
    private MovementInfo movementInfo;

    private GamePlayApi gamePlayApi =>ApiManager.GetWebSoketApi<GamePlayApi>();
    private PlayerMovementApi playerMovementApi => ApiManager.GetUdpApi<PlayerMovementApi>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementInfo ??= new MovementInfo();
        movementInfo.Account = account;

        trigger = gameObject.GetOrAddComponent<EventTrigger>();
        trigger.RemoveAllEventListener();
        trigger.AddTriggerEventListener(EventTriggerType.BeginDrag, OnMoveBegin);
        trigger.AddTriggerEventListener(EventTriggerType.Drag, OnMoving);
        trigger.AddTriggerEventListener(EventTriggerType.EndDrag, OnMoveEnd);

        if (targetRect == null) targetRect = transform.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();
    }
    private void OnMoveEnd(PointerEventData data)
    {
        movementInfo.EOpState = EOperationState.Finish;
        movementInfo.X = targetRect.anchoredPosition.x;
        movementInfo.Y = targetRect.anchoredPosition.y;
        gamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_SetMovementState, movementInfo);
    }

    private void OnMoving(PointerEventData data)
    {
        // 将屏幕坐标转换为 Canvas 空间坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, data.position, data.pressEventCamera, out Vector2 localPoint);
        // 设置 UI 元素的 anchoredPosition
        targetRect.anchoredPosition = localPoint + dragOffset;

        movementInfo.EOpState = EOperationState.InProgress;
        movementInfo.X = targetRect.anchoredPosition.x;
        movementInfo.Y = targetRect.anchoredPosition.y;
        movementInfo.MoveLerpSpeed = moveLerpSpeed;
        playerMovementApi.SendUdpMessage(NetworkEventPaths.PlayerMovement_MoveInGame, movementInfo);
    }

    private void OnMoveBegin(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, data.position, data.pressEventCamera, out Vector2 localPoint);
        dragOffset = targetRect.anchoredPosition - localPoint;

        movementInfo.EOpState = EOperationState.Begin;
        movementInfo.X = targetRect.anchoredPosition.x;
        movementInfo.Y = targetRect.anchoredPosition.y;
        gamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_SetMovementState, movementInfo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
