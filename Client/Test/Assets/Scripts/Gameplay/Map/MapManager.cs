using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public int width;
    public int height;
    public CinemachineCamera cinemachineCamera;
    public Transform plantRoot;
    public Tilemap tilemap;
    public TileBase tileGround;
    public float lacunarity = 0.1f;
    public int seed;
    public bool useNewSeed = false;

    [Range(0, 1)]
    public float threshold = 0.3f;
    public List<PlantConfig> plantConfigs;

    public static MapManager Instance { private set; get; }

    private MapQuadTree mapQuadTree;
    private Dictionary<Vector2Int, MapObstacleObj> obstracleDic = new();
    private float[,] groundTileNoiseValue;
    private BitArray obstacleBitMap;
    private System.Random random;

    private Vector3 lastUpdatePos;
    private int minLeafSize = 8;
    public MapManager()
    {
        Instance = this;
    }
    private void Start()
    {
        GenerateMapBySeed(seed);
    }
    public void GenerateMapBySeed(int seed)
    {
        this.seed = seed;
        useNewSeed = false;

        ClearTileMap();
        InitMapData();
        GenerateMap();

        BuildQuadTree();
        UpdateVisibleNodes();
    }
    public void GenerateMap()
    {
        ClearTileMap();
        InitMapData();
        SetTileMap();
        GeneratePlant();

        BuildQuadTree();
        UpdateVisibleNodes();
    }
    private bool showPlant = false;
    public void ShowAllPlant()
    {
        showPlant = !showPlant;
        foreach (var plant in obstracleDic)
        {
            plant.Value.SetVisible(showPlant, true);
        }
    }
    private void BuildQuadTree()
    {
        mapQuadTree ??= new();
        mapQuadTree.ClearTree();
        mapQuadTree.SetMapSize(width, height);
        mapQuadTree.SetObstacleBitMap(obstacleBitMap);

        int mapSize = Mathf.NextPowerOfTwo(Mathf.Max(width, height));

        mapQuadTree.BuildQuadTree(0, 0, mapSize, minLeafSize);
    }
    public void UpdateVisibleNodes()
    {
        Vector3 camPos = cinemachineCamera.transform.position;
        if (lastUpdatePos == Vector3.zero) lastUpdatePos = camPos;

        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        Vector2 damping = cinemachineCamera.GetComponent<CinemachinePositionComposer>().Damping;
        float cellSize = 1f; // 每个格子世界大小
        float marginX = damping.x * 2f * cellSize;
        float marginY = damping.y * 2f * cellSize;
        float margin = Mathf.Max(marginX, marginY);

        // 世界坐标矩形 → 转成数组坐标（加上偏移）
        float offsetX = 0.5f * width;
        float offsetY = 0.5f * height;

        float left = camPos.x - halfWidth - margin + offsetX;
        float bottom = camPos.y - halfHeight - margin + offsetY;
        float right = camPos.x + halfWidth + margin + offsetX;
        float top = camPos.y + halfHeight + margin + offsetY;

        int minX = Mathf.FloorToInt(left);
        int minY = Mathf.FloorToInt(bottom);
        int maxX = Mathf.FloorToInt(right);
        int maxY = Mathf.FloorToInt(top);

        RectInt viewRect = new(xMin: minX, xMax: maxX, yMin: minY, yMax: maxY);

        mapQuadTree.UpdateVisibleNodes(viewRect);
    }

    public MapObstacleObj GetObstacleAt(int x, int y)
    {
        Vector2Int index = new(x, y);
        obstracleDic.TryGetValue(index, out MapObstacleObj result);
        return result;
    }
    private bool IsGround(Vector2Int index) 
    {
        if (groundTileNoiseValue == null) return false;

        int x = index.x;
        int y = index.y;
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        bool isGround = groundTileNoiseValue[x, y] <= threshold;
        return isGround;
    }
    public bool IsObstacle(Vector2 vec2Pos)
    {
        if (tileGround == null || obstacleBitMap == null) return true;

        int x = Mathf.RoundToInt(vec2Pos.x + 0.5f * width);
        int y = Mathf.RoundToInt(vec2Pos.y + 0.5f * height);

        // 检查边界
        if (x < 0 || x >= width || y < 0 || y >= height)
            return true; // 超出地图也算障碍

        // 一维索引
        int index = y * width + x;

        return obstacleBitMap[index];
    }
    public bool IsEmptyTile(Vector2 vec2Pos) 
    {
        // 将世界坐标映射回数组索引
        int x = Mathf.RoundToInt(vec2Pos.x + 0.5f * width);
        int y = Mathf.RoundToInt(vec2Pos.y + 0.5f * height);

        bool isGround = IsGround(new Vector2Int(x, y));
        return !isGround;
    }
    private bool IsEdgeTileOrEmpty(Vector2Int index)
    {
        bool isGround = IsGround(index);
        if (!isGround) return true;

        // 判断是否是边缘格子（周围有空格）
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = index.x + dx;
                int ny = index.y + dy;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) return true; // 边缘
                if (groundTileNoiseValue[nx, ny] > threshold) return true; // 周围是空格，说明是边缘
            }
        }

        return false; // 不是空格，也不是边缘
    }
    public bool IsEdgeTileOrEmpty(Vector2 vec2Pos)
    {
        if (groundTileNoiseValue == null) return true;

        // 将世界坐标映射回数组索引
        int x = Mathf.RoundToInt(vec2Pos.x + 0.5f * width);
        int y = Mathf.RoundToInt(vec2Pos.y + 0.5f * height);

        // 检查边界
        if (x < 0 || x >= width || y < 0 || y >= height) return true;

        // 判断该格子是否是地面
        bool isGround = groundTileNoiseValue[x, y] <= threshold;

        // 如果不是地面（空格）或者处于边缘，就返回 true
        if (!isGround) return true;

        // 可选：判断是否是边缘格子（周围有空格）
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) return true; // 边缘
                if (groundTileNoiseValue[nx, ny] > threshold) return true; // 周围是空格，说明是边缘
            }
        }

        return false; // 不是空格，也不是边缘
    }
    private void GeneratePlant()
    {
        plantRoot.RemoveChildren();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = groundTileNoiseValue[x, y];
                bool isGround = groundTileNoiseValue[x, y] <= threshold;
                if (!isGround) continue;

                float chance = random.Next(0, 1000) * 0.001f;
                Vector2Int index = new Vector2Int(x, y);

                bool isEdge = IsEdgeTileOrEmpty(index);
                if (isEdge) continue;

                foreach (var plantConf in plantConfigs)
                {
                    (bool isGenPlant, var mapObstacleObj) = plantConf.GenPlant(noise, index, chance);
                    obstacleBitMap[y * width + x] = isGenPlant;
                    if (mapObstacleObj != null) obstracleDic.Add(index, mapObstacleObj);
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
                groundTileNoiseValue[x, y] = noise;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = groundTileNoiseValue[x, y];
                groundTileNoiseValue[x, y] = Mathf.InverseLerp(nosieMin, nosieMax, noise);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isGround = groundTileNoiseValue[x, y] <= threshold;
                int posX = Mathf.CeilToInt(x - 0.5f * width);
                int posY = Mathf.CeilToInt(y - 0.5f * height);
                if (isGround) tilemap.SetTile(new Vector3Int(posX, posY), tileGround);
            }
        }
    }
    private void InitMapData()
    {
        groundTileNoiseValue = new float[width, height];
        if (useNewSeed) seed = DateTimeOffset.UtcNow.ToUnixTimeSeconds().GetHashCode();
        random = new(seed);

        foreach (var plant in plantConfigs)
        {
            plant.SetPlantRoot(plantRoot);
            plant.SetGroundThreshold(threshold);
            plant.SetMapSize(width, height);
        }

        obstacleBitMap = new BitArray(width * height);
    }

    private void Update()
    {
        Vector3 camPos = cinemachineCamera.transform.position;
        if (Vector3.Magnitude(camPos - lastUpdatePos) >= minLeafSize * 0.25f)
        {
            lastUpdatePos = camPos;
            UpdateVisibleNodes();
        }
    }
    public void ClearTileMap()
    {
        mapQuadTree?.ClearTree();
        obstracleDic.Clear();
        tilemap.ClearAllTiles();
        plantRoot.RemoveChildren();
    }
}
[Serializable]
public class MapGenerator 
{
    public class MapGeneratorData 
    {
        public int width;
        public int height;
        public float lacunarity;    //频率
        public float threshold;     //阈值
        public int seed;
    }

    public List<ScriptableObject> spawnConfigs;

    public float[,] NoiseValue { get { return noiseValue; } }
    private float[,] noiseValue;

    private System.Random random;

    private int width;
    private int height;
    private float lacunarity;    //频率
    private float threshold;     //阈值
    private int seed;

    private MapGeneratorData generatorData;
    
    public MapGenerator(MapGeneratorData data) 
    {
        generatorData = data;

        width = generatorData.width;
        height = generatorData.height;
        lacunarity = generatorData.lacunarity;
        threshold = generatorData.threshold;
        seed = generatorData.seed;

        noiseValue = new float[width, height];
        random = new(seed);

    }
}
