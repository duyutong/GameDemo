using NUnit.Framework;
using UnityEngine;

public class MapSpawnObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
   
    public int indexX;
    public int indexY;
    public float spawntNoise;
    private GameObject main;
    private bool currVisible;
    public void SetVisible(bool isVisible,bool isInit = false) 
    {
        if (main == null) main = transform.GetChild(0).gameObject;
        
        if (isInit) currVisible = isVisible;
        main.SetActive(isVisible);

        if (!isInit && isVisible != currVisible) 
        {
            currVisible = isVisible;
            // 开始变化
        }
    }
    public void SetIndexOnMap(Vector2Int index,float noise) 
    {
        indexX = index.x; 
        indexY = index.y;
        spawntNoise = noise;
    }
    public (int x,int y) GetIndexOnMap() 
    {
        return (indexX, indexY);
    }
}
