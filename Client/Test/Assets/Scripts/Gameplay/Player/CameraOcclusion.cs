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

    private Camera cam;

    // 当前帧命中的
    private HashSet<SpriteRenderer> currentHits = new HashSet<SpriteRenderer>();
    // 上一帧命中的
    private HashSet<SpriteRenderer> lastHits = new HashSet<SpriteRenderer>();

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        currentHits.Clear();

        Vector3[] corners = GetCorners();
        Vector3 start = cam.transform.position;
        start.z = 0;

        foreach (var corner in corners)
        {
            Vector3 end = corner;
            end.z = 0;

            var hits = Physics2D.LinecastAll(start, end, checkLayer);
            foreach (var h in hits)
            {
                if (h.collider.TryGetComponent<SpriteRenderer>(out var sr))
                {
                    if (sr.sortingOrder < playerRenderer.sortingOrder) continue;
                    currentHits.Add(sr);
                }
            }
        }

        // ⭐ 恢复不再命中的
        foreach (var r in lastHits)
        {
            if (r != null && !currentHits.Contains(r)) SetAlpha(r, 1f);
        }

        // ⭐ 设置当前命中的透明
        foreach (var r in currentHits)
        {
            if (r == null) continue;
            SetAlpha(r, fadeAlpha);
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