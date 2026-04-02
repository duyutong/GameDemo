using System.Collections.Generic;
using UnityEngine;

public class CameraOcclusion : MonoBehaviour
{
    [Header("Player Size")]
    public float width = 1f;
    public float height = 1f;

    [Header("Occlusion")]
    public LayerMask checkLayer;
    public float fadeAlpha = 0.3f;

    [Header("SpriteRenderer")]
    public SpriteRenderer playerRenderer;

    private ContactFilter2D filter = new ContactFilter2D();
    private RaycastHit2D[] hits = new RaycastHit2D[16];
    private Camera cam;

    // 当前帧命中的
    private HashSet<MapSpawnObj> currentHits = new HashSet<MapSpawnObj>();
    // 上一帧命中的
    private HashSet<MapSpawnObj> lastHits = new HashSet<MapSpawnObj>();

    void Start()
    {
        cam = Camera.main;

        filter.SetLayerMask(checkLayer);
        filter.useTriggers = true;
    }

   private void LateUpdate()
    {
        currentHits.Clear();

        Vector3[] corners = GetCorners();
        Vector3 start = cam.transform.position;
        start.z = 0;

        foreach (var corner in corners)
        {
            Vector3 end = corner;
            end.z = 0;

           
            int count = Physics2D.Linecast(start, end, filter, hits);

            for (int i = 0; i < count; i++) 
            {
                var h = hits[i];
                var col = h.collider;

                if (!col) continue;
                if (!col.transform.parent.TryGetComponent<MapSpawnObj>(out var spawn)) continue;

                SpriteRenderer sr = spawn.spriteRenderer;
                if (sr.sortingOrder < playerRenderer.sortingOrder) continue;

                currentHits.Add(spawn);
            }
        }

        // ⭐ 恢复不再命中的
        foreach (var spawn in lastHits)
        {
            if (spawn != null && !currentHits.Contains(spawn)) 
            {
                spawn.SetCameraOcclusionHit(false);
            } 
        }

        // ⭐ 设置当前命中的透明
        foreach (var spawn in currentHits)
        {
            if (spawn == null) continue;
            spawn.SetCameraOcclusionHit(true);
        }

        // ⭐ 交换缓存
        lastHits.Clear();
        foreach (var r in currentHits) lastHits.Add(r);
    }

    // 获取玩家矩形四个角
   private Vector3[] GetCorners()
    {
        Vector3 center = transform.position;

        float halfW = width * 0.5f;

        return new Vector3[]
        {
            center + new Vector3(-halfW,  height, 0),
            center + new Vector3( halfW,  height, 0),
            center + new Vector3(-halfW, 0, 0),
            center + new Vector3( halfW, 0, 0),
        };
    }

   private void SetAlpha(SpriteRenderer sr, float alpha)
    {
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}