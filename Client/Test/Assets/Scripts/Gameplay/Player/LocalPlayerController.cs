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
        //int height = MapManager.Instance.GetMapDataByNameByLayer(EMapLayer.BaseDround).height;
        //int layoutOrder = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, transform.position.y) * height);
        //layoutOrder *= 10;
        //layoutOrder += 1;
        //playerRenderer.sortingOrder = layoutOrder;

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
}
