using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MapGenerator
{
    private float[,] noiseValue;
    private BitArray spawnBitMap;
    private MapGeneratorData generatorData;

    private Tilemap tilemap;
    private TileBase tileGround;

    private int width;
    private int height;
    private EMapPreferencePosition prefabPos;
    private float lacunarity;    //频率
    private Vector2 threshold;     //阈值
    private int seed;
    private MapSpawnGenData spawns;

    private MapQuadTree mapQuadTree;
    private System.Random random;

    private MapSpawnGenerator spawnGenerator;
    private Dictionary<Vector2Int, MapSpawnObj> spawnDic = new();
    private CinemachineCamera cmCam;
    private CinemachinePositionComposer cmPosComposer;
    private float targetNoise = -1;
    private Vector3 lastUpdatePos;

    public bool InitFinshi { get; private set; }
    public MapGeneratorData GeneratorData { get { return generatorData; } }
    public MapGenerator(MapGeneratorData data)
    {
        generatorData = data;
        tilemap = generatorData.tilemap;
        spawns = generatorData.spawns;
        spawnGenerator ??= new MapSpawnGenerator();
    }

    public void GenerateMap()
    {
        ClearMap();

        InitMapData();
        SetTileMap();
        GenerateSpawn();

        BuildQuadTree();
        UpdateVisibleNodes();

        InitFinshi = true;
    }
    public void ClearMap()
    {
        InitFinshi = false;

        targetNoise = -1;
        mapQuadTree?.ClearTree();
        spawnDic.Clear();
        tilemap.ClearAllTiles();
        spawns.Clear();
    }
    private void InitMapData()
    {
        tilemap = generatorData.tilemap;
        tileGround = generatorData.tileGround;

        width = generatorData.width;
        height = generatorData.height;
        prefabPos = generatorData.position;
        lacunarity = generatorData.lacunarity;
        threshold = generatorData.threshold;
        seed = generatorData.seed;
        spawns = generatorData.spawns;

        noiseValue = new float[width, height];
        if (generatorData.useNewSeed)
        {
            seed = DateTimeOffset.UtcNow.ToUnixTimeSeconds().GetHashCode();
            generatorData.seed = seed;
        }
        random = new(seed);
    }
    private Vector2 GetSpawnsThreshold() 
    {
        if (prefabPos == EMapPreferencePosition.Random) return threshold;
        else 
        {
            if (targetNoise <= -1) targetNoise = SetPosTargetNosie();
            float maxOffset = Mathf.Abs(threshold.y - threshold.x);
            float half = maxOffset * 0.5f;
            float min = targetNoise - half;
            float max = targetNoise + half;
            // 如果超出边界，就整体平移回来
            if (min < 0) { max -= min; min = 0; }
            if (max > 1) { min -= (max - 1); max = 1; }

            min = Mathf.Clamp01(min);
            max = Mathf.Clamp01(max);

            return new Vector2(min, max);
        }
    }
    private float SetPosTargetNosie()
    {
        int x = 0, y = 0;
        switch (prefabPos)
        {
            case EMapPreferencePosition.BottomLeft:
                x = 0; y = 0; break;
            case EMapPreferencePosition.BottomCenter:
                x = (width - 1) / 2; y = 0; break;
            case EMapPreferencePosition.BottomRight:
                x = width - 1; y = 0; break;
            case EMapPreferencePosition.MiddleLeft:
                x = 0; y = (height - 1) / 2; break;
            case EMapPreferencePosition.Center:
                x = (width - 1) / 2; y = (height - 1) / 2; break;
            case EMapPreferencePosition.MiddleRight:
                x = width - 1; y = (height - 1) / 2; break;
            case EMapPreferencePosition.TopLeft:
                x = 0; y = height - 1; break;
            case EMapPreferencePosition.TopCenter:
                x = (width - 1) / 2; y = height - 1; break;
            case EMapPreferencePosition.TopRight:
                x = width - 1; y = height - 1; break;
        }

        float noise = noiseValue[x, y];
        if (noise == 0) Debug.LogError("aaaa???");
        return noise;
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
                noiseValue[x, y] = noise;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = noiseValue[x, y];
                noiseValue[x, y] = Mathf.InverseLerp(nosieMin, nosieMax, noise);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isGround = IsGround(new Vector2Int(x, y));
                int posX = Mathf.CeilToInt(x - 0.5f * width);
                int posY = Mathf.CeilToInt(y - 0.5f * height);
                if (isGround) tilemap.SetTile(new Vector3Int(posX, posY), tileGround);
            }
        }
    }
    private void GenerateSpawn()
    {
        Vector2 spawnsThreshold = GetSpawnsThreshold();
        spawnGenerator.SetSpawnObjRoot(spawns.root);
        spawnGenerator.SetGroundThreshold(spawnsThreshold);
        spawnGenerator.SetMapSize(width, height);

        spawns.Clear();
        spawnBitMap = new BitArray(width * height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = noiseValue[x, y];
                bool isGround = IsGround(new Vector2Int(x, y));
                if (!isGround) continue;

                float chance = random.Next(0, 1000) * 0.001f;
                Vector2Int index = new Vector2Int(x, y);

                bool isEdge = IsEdgeTileOrEmpty(index);
                if (isEdge) continue;

                foreach (var spawnConf in spawns.confs)
                {
                    (bool isGenPlant, var mapSpawnObj) = spawnGenerator.GenSpawnObj(noise, index, chance,spawnConf);
                    spawnBitMap[y * width + x] = isGenPlant;
                    if (mapSpawnObj != null) spawnDic.Add(index, mapSpawnObj);
                    if (isGenPlant) break;
                }
            }
        }

    }
    private void BuildQuadTree()
    {
        mapQuadTree ??= new();
        mapQuadTree.ClearTree();
        mapQuadTree.SetMapSize(width, height);
        mapQuadTree.SetSpawnBitMap(spawnBitMap);

        int mapSize = Mathf.NextPowerOfTwo(Mathf.Max(width, height));
        int minLeafSize = 8;
        mapQuadTree.BuildQuadTree(0, 0, mapSize, minLeafSize);
    }
   
    public void Update()
    {
        if (!InitFinshi) return;

        if (generatorData.spawns.confs != null && generatorData.spawns.confs.Count > 0) 
        {
            cmCam = cmCam != null ? cmCam : MapManager.Instance.cmCam;
            Vector3 camPos = cmCam.transform.position;
            if (Vector3.Magnitude(camPos - lastUpdatePos) >= 2)
            {
                lastUpdatePos = camPos;
                UpdateVisibleNodes();
            }
        }
    }
    private void UpdateVisibleNodes()
    {
        cmCam = cmCam != null ? cmCam : MapManager.Instance.cmCam;
        Vector3 camPos = cmCam.transform.position;
        if (lastUpdatePos == Vector3.zero) lastUpdatePos = camPos;

        Camera cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        cmPosComposer = cmPosComposer != null ? cmPosComposer : cmCam.GetComponent<CinemachinePositionComposer>();
        Vector2 damping = cmPosComposer.Damping;
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
        mapQuadTree.UpdateVisibleNodes<MapSpawnObj>(viewRect, (_isVisible, _obj) => _obj.SetVisible(_isVisible));
    }
    private Vector2Int WorldPosToMapIndex(Vector2 worldPos)
    {
        // 将世界坐标映射回数组索引
        int x = Mathf.RoundToInt(worldPos.x + 0.5f * width);
        int y = Mathf.RoundToInt(worldPos.y + 0.5f * height);

        return new Vector2Int(x, y);
    }
    private int WorldPosToBitMapIndex(Vector2 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x + 0.5f * width);
        int y = Mathf.RoundToInt(worldPos.y + 0.5f * height);

        int index = y * width + x;
        return index;
    }
    public MapSpawnObj GetSpawnObjAt(Vector2Int index)
    {
        spawnDic.TryGetValue(index, out MapSpawnObj result);
        return result;
    }
    public bool IsObstacle(Vector2 worldPos)
    {
        if (tileGround == null || spawnBitMap == null) return true;

        int index = WorldPosToBitMapIndex(worldPos);
        return spawnBitMap[index];
    }
    public bool IsEmptyTile(Vector2 worldPos)
    {
        bool isGround = IsGround(WorldPosToMapIndex(worldPos));
        return !isGround;
    }
    private bool IsGround(Vector2Int index)
    {
        int x = index.x;
        int y = index.y;
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        if (prefabPos == EMapPreferencePosition.Random)
        {
            bool isGround = threshold.x < noiseValue[x, y] && noiseValue[x, y] <= threshold.y;
            return isGround;
        }
        else
        {
            if (targetNoise <= -1) targetNoise = SetPosTargetNosie();
            float maxOffset = Mathf.Abs(threshold.y - threshold.x);
            float half = maxOffset * 0.5f;
            float min = targetNoise - half;
            float max = targetNoise + half;
            // 如果超出边界，就整体平移回来
            if (min < 0) { max -= min; min = 0; }
            if (max > 1) { min -= (max - 1); max = 1; }

            min = Mathf.Clamp01(min);
            max = Mathf.Clamp01(max);

            if(x ==0 && y == 0) Debug.Log($" {min} < {targetNoise} <= {max}");
            bool isGround = min < noiseValue[x, y] && noiseValue[x, y] <= max;
            return isGround;
        }
    }
    private bool IsObstacle(Vector2Int index)
    {
        if (tileGround == null || spawnBitMap == null) return true;

        int x = index.x;
        int y = index.y;

        int obstacleIndex = y * width + x;
        return spawnBitMap[obstacleIndex];
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
                if (noiseValue[nx, ny] > threshold.y || noiseValue[nx, ny] < threshold.x) return true; // 周围是空格，说明是边缘
            }
        }

        return false; // 不是空格，也不是边缘
    }
}
[Serializable]
public class MapSpawnGenData
{
    public Transform root;
    public List<MapSpawnConfig> confs = new();
    public void Clear()
    {
        if (root != null) root.RemoveChildren();
    }
}
