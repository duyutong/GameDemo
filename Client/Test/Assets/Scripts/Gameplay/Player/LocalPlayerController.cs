using UnityEngine;
using static EnumDefinitions;
using static UnityEditor.PlayerSettings;

public class LocalPlayerController : MonoBehaviour
{
    public SpriteRenderer playerRenderer;
    public Animator playerAnimator;

    private EOperationState lastState = EOperationState.None;
    public void OnPlayerMove(EOperationState state, Vector2 linearVelocity)
    {
        SetSortingOrder();
        playerRenderer.flipX = linearVelocity.x < 0;
        if (lastState != state)
        {
            lastState = state;

            if (state != EOperationState.InProgress) playerAnimator.Play("idle-down");
            else
            {
                if (linearVelocity.y > 0 && linearVelocity.x == 0) { playerAnimator.Play("idle-up"); return; }
                if (linearVelocity.y < 0 && linearVelocity.x == 0) { playerAnimator.Play("idle-down"); return; }
                playerAnimator.Play("idle-side");
            }
        }
    }

    private void SetSortingOrder()
    {
        if (MapManager.Instance != null)
        {
            var mapGenerator = MapManager.Instance.GetMapDataByNameByLayer(EMapLayer.BaseGround);
            if (mapGenerator == null) return;
            int height = mapGenerator.height;
            int layoutOrder = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, transform.position.y) * height);
            layoutOrder *= 10;
            layoutOrder += 1;
            playerRenderer.sortingOrder = layoutOrder;
        }
    }
}
