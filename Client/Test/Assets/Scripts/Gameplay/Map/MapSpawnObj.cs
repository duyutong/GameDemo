using NUnit.Framework;
using UnityEngine;

public class MapSpawnObj : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
   
    private int X;
    private int Y;
    private GameObject main;
    private bool currVisible;
    public void SetVisible(bool isVisible,bool isInit = false) 
    {
        if (main == null) main = transform.GetChild(0).gameObject;
        
        if (isInit) currVisible = isVisible;
        main.SetActive(isVisible);

        if (!isInit && isVisible != currVisible) 
        {
            // 开始变化
        }
    }
    public void SetIndexOnMap(Vector2Int index) 
    {
        X = index.x; Y = index.y;
    }
    public (int x,int y) GetIndexOnMap() 
    {
        return (X, Y);
    }
}
