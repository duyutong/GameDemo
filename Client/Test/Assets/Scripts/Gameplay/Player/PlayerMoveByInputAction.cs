using Network.Core.Frame;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static EnumDefinitions;
public class PlayerMoveByInputAction : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveLerpSpeed = 30;
    public LocalPlayerMovement playerMovement;

    private float syncInterval = 0.05f;
    private float syncTimer = 0f;

    private bool isMoveStartde = false;
    private Vector2 moveInput;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private InputAction moveAction;
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
        playerMovement?.SyncLocalPlayerMovement(EOperationState.Begin, transform.position, moveLerpSpeed);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoveStartde = false;
        rb.linearVelocity = Vector2.zero;
        playerMovement?.SyncLocalPlayerMovement(EOperationState.Finish, transform.position, moveLerpSpeed);
    }
    private void FixedUpdate()
    {
        if (isMoveStartde)
        {
            moveInput = moveAction.ReadValue<Vector2>();

            Vector2 move = moveInput.normalized;
            Vector2 velocity = moveSpeed * move;
            rb.linearVelocity = velocity;

            syncTimer -= Time.fixedDeltaTime;
            if (syncTimer <= 0)
            {
                syncTimer = syncInterval;
                playerMovement?.SyncLocalPlayerMovement(EOperationState.InProgress, transform.position, moveLerpSpeed);
            }
        }
    }
}
