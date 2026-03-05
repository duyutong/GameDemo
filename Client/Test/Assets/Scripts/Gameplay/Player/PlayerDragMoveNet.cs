using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static EnumDefinitions;

public class PlayerDragMoveNet : MonoBehaviour
{
    public string account = "ABC";
    public float moveLerpSpeed = 30;
    public LocalPlayerMovement playerMovement;

    private EventTrigger trigger;
    private RectTransform targetRect;
    private RectTransform parentRect;
    private Vector2 dragOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        playerMovement.SyncLocalPlayerMovement(EOperationState.Finish, targetRect.position, moveLerpSpeed);
    }

    private void OnMoving(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, data.position, data.pressEventCamera, out Vector2 localPoint);
        targetRect.anchoredPosition = localPoint + dragOffset;

        playerMovement.SyncLocalPlayerMovement(EOperationState.InProgress, targetRect.position, moveLerpSpeed);
    }

    private void OnMoveBegin(PointerEventData data)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, data.position, data.pressEventCamera, out Vector2 localPoint);
        dragOffset = targetRect.anchoredPosition - localPoint;

        playerMovement.SyncLocalPlayerMovement(EOperationState.Begin, targetRect.position, moveLerpSpeed);
    }
}
