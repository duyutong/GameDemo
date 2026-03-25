using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public Transform plantRoot;
    public Tilemap tilemap;
    public TileBase tileGround;
    public float lacunarity = 0.1f;
    public int seed;
    public bool useNewSeed = false;

    public static MapGenerator Instance { private set; get; }

    [Range(0, 1)]
    public float threshold = 0.3f;

    public List<PlantConfig> plantConfigs;

    private float[,] tileNoiseValue;
    private System.Random random;
    private Vector3Int[] edgeCheckDirs = new Vector3Int[]
       {
            Vector3Int.up,             // 上
            Vector3Int.down,           // 下
            Vector3Int.left,           // 左
            Vector3Int.right,          // 右
            new Vector3Int(1, 1, 0),  // 右上
            new Vector3Int(1, -1, 0), // 右下
            new Vector3Int(-1, 1, 0), // 左上
            new Vector3Int(-1, -1, 0) // 左下
       };

    public void Awake()
    {
        Instance = this;
    }
    public void GenerateMap()
    {
        InitMapData();
        SetTileMap();
        GeneratePlant();
    }
    public bool IsEdgeTileOrEmpty(Vector2 vec2Pos)
    {
        if (tileGround == null) return true;

        int x = Mathf.RoundToInt(vec2Pos.x);
        int y = Mathf.RoundToInt(vec2Pos.y);

        Vector3Int pos = new Vector3Int(x, y);

        // 当前这个位置必须有 Tile
        if (!tilemap.HasTile(pos)) return true;

        // 只要有一个方向没有 Tile，就是边缘
        foreach (var dir in edgeCheckDirs)
        {
            Vector3Int neighbor = pos + dir;
            if (!tilemap.HasTile(neighbor)) return true;
        }

        return false;
    }
    private void GeneratePlant()
    {
        plantRoot.RemoveChildren();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = tileNoiseValue[x, y];
                bool isGround = tileNoiseValue[x, y] <= threshold;
                if (!isGround) continue;

                float chance = random.Next(0, 1000) * 0.001f;
                float posX = x - 0.5f * width;
                float posY = y - 0.5f * height;
                Vector2 pos = new(posX, posY);
                bool isEdge = IsEdgeTileOrEmpty(pos);
                if (isEdge) continue;

                foreach (var plant in plantConfigs)
                {
                    bool isGenPlant = plant.GenPlant(noise, pos, chance);
                    if (isGenPlant) break;
                }
            }
        }
    }
    private void SetTileMap()
    {
        float randomOffset = random.Next(-1000, 1000);
        float nosieMax = float.MinValue;
        float nosieMin = float.MaxValue;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * lacunarity + randomOffset, y * lacunarity + randomOffset);
                if (noise < nosieMin) nosieMin = noise;
                if (noise > nosieMax) nosieMax = noise;
                tileNoiseValue[x, y] = noise;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = tileNoiseValue[x, y];
                tileNoiseValue[x, y] = Mathf.InverseLerp(nosieMin, nosieMax, noise);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isGround = tileNoiseValue[x, y] <= threshold;
                int posX = Mathf.CeilToInt(x - 0.5f * width);
                int posY = Mathf.CeilToInt(y - 0.5f * height);
                if (isGround) tilemap.SetTile(new Vector3Int(posX, posY), tileGround);
            }
        }
    }
    private void InitMapData()
    {
        tileNoiseValue = new float[width, height];
        if (useNewSeed) seed = DateTimeOffset.UtcNow.ToUnixTimeSeconds().GetHashCode();
        random = new(seed);

        foreach (var plant in plantConfigs)
        {
            plant.SetPlantRoot(plantRoot);
            plant.SetGroundThreshold(threshold);
            plant.SetGroundSize(width, height);
        }
    }
    public void ClearTileMap()
    {
        tilemap.ClearAllTiles();
        plantRoot.RemoveChildren();
    }
}
[Serializable]
public class PlantRule
{
    public string name;
    public float min;
    public float max;
}
