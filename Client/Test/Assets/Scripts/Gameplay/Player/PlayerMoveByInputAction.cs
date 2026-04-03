using Network.Core.Frame;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static EnumDefinitions;
public class PlayerMoveByInputAction : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveLerpSpeed = 30;
    public LocalPlayerMovement playerMovement;
    public MovementEvent movementEevent;

    private float syncInterval = 0.05f;
    private float syncTimer = 0f;

    private bool isMoveStartde = false;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private InputAction moveAction;

    private MapGenerator baseGround;
    private void Awake()
    {
        controls = new PlayerControls();  // 生成类实例
        moveAction = controls.Player.Move; // Player 是 Action Map 名，Move 是 Action 名

        rb = rb != null ? rb : gameObject.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.started += OnMoveStarted;
        moveAction.canceled += OnMoveCanceled;
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.started -= OnMoveStarted;
        moveAction.canceled -= OnMoveCanceled;
        moveAction.Disable();
    }

    private void OnMoveStarted(InputAction.CallbackContext context)
    {
        syncInterval = FrameManager.Instance.FrameSyncIntervalMs * 0.001f;
        isMoveStartde = true;

        movementEevent?.Invoke(EOperationState.Begin, rb.linearVelocity);
        playerMovement?.SyncLocalPlayerMovement(EOperationState.Begin, transform.position, moveLerpSpeed);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoveStartde = false;
        rb.linearVelocity = Vector2.zero;

        movementEevent?.Invoke(EOperationState.Finish, rb.linearVelocity);
        playerMovement?.SyncLocalPlayerMovement(EOperationState.Finish, transform.position, moveLerpSpeed);
    }
    private bool CheckMove(Vector2 checkPos)
    {
        if (MapManager.Instance == null) return true;

        baseGround ??= MapManager.Instance.GetMapByLayer(EMapLayerType.BaseGround);
        if (baseGround == null) return true;

        bool IsEdgeTileOrEmpty = MapManager.Instance.IsEmptyTile(checkPos);
        if (IsEdgeTileOrEmpty) return false;

        return true;
    }
    private void FixedUpdate()
    {
        if (isMoveStartde)
        {
            moveInput = moveAction.ReadValue<Vector2>();

            Vector2 move = moveInput.normalized;
            Vector2 velocity = moveSpeed * move;

            float dt = Time.fixedDeltaTime;
            Vector2 checkPos = rb.position + velocity * dt * 10 - 0.5f * Vector2.right;
            rb.linearVelocity = CheckMove(checkPos) ? velocity : Vector2.zero;

            syncTimer -= Time.fixedDeltaTime;
            if (syncTimer <= 0)
            {
                syncTimer = syncInterval;
                movementEevent?.Invoke(EOperationState.InProgress, velocity);
                playerMovement?.SyncLocalPlayerMovement(EOperationState.InProgress, transform.position, moveLerpSpeed);
            }
        }
    }
}
[Serializable]
public class MovementEvent : UnityEvent<EOperationState, Vector2> { }