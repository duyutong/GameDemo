using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractableTileItem : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Tile Reference")]
    [SerializeField] private Tilemap itemTilemap;   // 道具所在的那张 Tilemap（共享）
    [SerializeField] private Vector3Int cellPos;    // 这个道具占用的格子坐标（每个代理不一样）

    [Header("Prompt Auto-Bind")]
    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1f, 0f);
    private GameObject promptInstance;

    [Header("Highlight")]
    [SerializeField] private float brightenMultiplier = 1.2f;

    private bool playerInRange;
    private Color originalTilemapColor;

   private void Awake()
{
    // 1. 记录 Tilemap 原始颜色（用于高亮恢复）
    if (itemTilemap != null)
        originalTilemapColor = itemTilemap.color;

    // 2. 自动根据“代理物体的位置”计算它对应的格子坐标
    if (itemTilemap != null)
    {
        cellPos = itemTilemap.WorldToCell(transform.position);
    }

    // 3. 自动生成提示 UI
    if (promptPrefab != null)
    {
        promptInstance = Instantiate(promptPrefab, transform);
        promptInstance.name = "Prompt";
        promptInstance.transform.localPosition = promptOffset;
        promptInstance.SetActive(false);
    }
}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
        SetPrompt(true);
        SetHighlight(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        SetPrompt(false);
        SetHighlight(false);
    }

    private void OnMouseDown()
    {
        if (!playerInRange) return;
        Interact();
    }

    private void Interact()
    {
        if (itemTilemap == null) return;

        // 只删除这个道具对应的那一格 tile
        itemTilemap.SetTile(cellPos, null);

        // 删除代理物体本身（不是删 Tilemap）
        Destroy(gameObject);
    }

    private void SetPrompt(bool on)
    {
        if (promptInstance != null) promptInstance.SetActive(on);
    }

    private void SetHighlight(bool on)
    {
        // 这里先给你一个最简单的“整体变亮”占位写法
        // 如果你想“只高亮这一格”，我下面会说怎么做（推荐 Overlay Tilemap）
        if (itemTilemap == null) return;

        if (on)
        {
            var c = originalTilemapColor;
            itemTilemap.color = new Color(c.r * brightenMultiplier, c.g * brightenMultiplier, c.b * brightenMultiplier, c.a);
        }
        else
        {
            itemTilemap.color = originalTilemapColor;
        }
    }
}
