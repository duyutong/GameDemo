using NUnit.Framework;
using UnityEngine;

public class MapObstacleObj : MonoBehaviour
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

        // �仯
        if (!isInit && isVisible != currVisible) 
        {
            //�����仯
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
