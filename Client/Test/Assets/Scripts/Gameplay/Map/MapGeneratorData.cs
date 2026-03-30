using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneratorData : MonoBehaviour
{
    public EMapLayer layer;
    public MapSpawnGenData spawns;

    public EMapPreferencePosition preferPos;
    [Range(0, 0.1f)] public float lacunarity;          //频率
    [MinMaxRangeSlider(0, 1)] public Vector2 threshold; //阈值

    [HideInInspector] public Tilemap tilemap;
    [HideInInspector] public TileBase tileGround;

    [HideInInspector] public int width;
    [HideInInspector] public int height;

    [HideInInspector] public int seed;
    [HideInInspector] public bool useNewSeed = false;

    private void ValidatePreferPos()
    {
        // 如果选了 Random，就清掉其他
        if ((preferPos & EMapPreferencePosition.Random) != 0)
        {
            preferPos = EMapPreferencePosition.Random;
            return;
        }

        // 如果没选任何，就给个默认
        if (preferPos == EMapPreferencePosition.None)
        {
            preferPos = EMapPreferencePosition.Center;
        }
    }
    private void OnValidate()
    {
        ValidatePreferPos();
    }
}
public class MapShapeParams
{
    public float centerX;
    public float centerY;

    public float a; // x轴半径
    public float b; // y轴半径

    public float warp;       // 扭曲强度
    public float warpScale;  // 扭曲频率

    public float seedX;      // 噪声偏移
    public float seedY;


    private System.Random random;
    public MapShapeParams(int seend)
    {
        random = new System.Random(seend);

        // 中心偏移
        centerX = 0.5f + (float)(random.NextDouble() * 0.2 - 0.1);
        centerY = 0.5f + (float)(random.NextDouble() * 0.2 - 0.1);

        // 椭圆形状
        a = (float)(0.6 + random.NextDouble() * 0.4); // 0.6 ~ 1
        b = (float)(0.5 + random.NextDouble() * 0.3); // 0.5 ~ 0.8

        // 扭曲
        warp = (float)(0.15 + random.NextDouble() * 0.25);     // 0.15 ~ 0.4
        warpScale = (float)(1.5 + random.NextDouble() * 2.5);  // 1.5 ~ 4

        // 噪声偏移（关键）
        seedX = (float)(random.NextDouble() * 2000 - 1000);
        seedY = (float)(random.NextDouble() * 2000 - 1000);
    }
}
[System.Flags]
public enum EMapPreferencePosition
{
    None = 0,

    // 单独使用（不能与其他组合）
    Random = 1 << 0,

    TopLeft = 1 << 1,
    TopCenter = 1 << 2,
    TopRight = 1 << 3,

    MiddleLeft = 1 << 4,
    Center = 1 << 5,
    MiddleRight = 1 << 6,

    BottomLeft = 1 << 7,
    BottomCenter = 1 << 8,
    BottomRight = 1 << 9,
}