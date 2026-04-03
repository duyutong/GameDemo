using NUnit.Framework;
using UnityEngine;

public class MapSpawnObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public BTRuntimeComponent bTRuntimeComp;

    private int indexX;
    private int indexY;
    private float spawntNoise;
    private GameObject main;
    private bool currVisible;
    public bool currCameraOcclusionHit;
    private const string onPlayerEnterStr = "TreeBT_Enter";
    private const string onPlayerExitStr = "TreeBT_Exit";
    public void SetVisible(bool isVisible)
    {
        if (main == null) main = transform.GetChild(0).gameObject;

        currVisible = isVisible;
        main.SetActive(currVisible);
        bTRuntimeComp?.SetEnable(currVisible);
    }
    public void SetCameraOcclusionHit(bool isHit, bool isInit = false)
    {
        if(isInit) currCameraOcclusionHit = isHit;
        if (currCameraOcclusionHit == isHit) return;

        currCameraOcclusionHit = isHit;
        bTRuntimeComp.SendMsgToBTRuntime(isHit ? onPlayerExitStr : onPlayerEnterStr, EBTState.中断);
        bTRuntimeComp.SendMsgToBTRuntime(isHit ? onPlayerEnterStr : onPlayerExitStr, EBTState.进入);
    }
    public void SetIndexOnMap(Vector2Int index, float noise)
    {
        indexX = index.x;
        indexY = index.y;
        spawntNoise = noise;
    }
    public (int x, int y) GetIndexOnMap()
    {
        return (indexX, indexY);
    }
}
