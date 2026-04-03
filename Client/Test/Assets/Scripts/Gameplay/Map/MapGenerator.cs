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
    private EMapPreferencePosition preferPos;
    private float lacunarity;    //频率
    private Vector2 threshold;     //阈值
    private int seed;
    private MapSpawnGenData spawns;
    private MapShapeParams shapeParams;
    private List<Vector2> allPreferPos = new();

    private MapQuadTree mapQuadTree;
    private System.Random random;

    private MapSpawnGenerator spawnGenerator;
    private Dictionary<Vector2Int, MapSpawnObj> spawnDic = new();
    private CinemachineCamera cmCam;
    private CinemachinePositionComposer cmPosComposer;
    private Vector3 lastUpdatePos;

    public bool InitFinshi { get; private set; }
    public float[,] NoiseValue { get { return (float[,])noiseValue.Clone(); } }
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
    public void ShowAllSpawn()
    {
        if (!InitFinshi) return;
        foreach (var spawn in spawnDic) spawn.Value.SetVisible(true);
    }
    public void ClearMap()
    {
        InitFinshi = false;

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
        preferPos = generatorData.preferPos;
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

        allPreferPos = GetAllPreferPositions();
        shapeParams = new(seed);

        spawnGenerator.SetMapSize(width, height);
        spawnGenerator.SetGroundThreshold(threshold.x, threshold.y);
        spawnGenerator.SetSpawnData(spawns);
        spawnGenerator.SetSeed(seed);
    }
    public Vector2 FindFarthestTile(Vector2 dirNormalized)
    {
        Vector2 center = new(width / 2f, height / 2f);
        Vector2 farthestTile = center; // 默认返回中心
        float maxDistance = 0f;
        float step = 1f;
        for (float t = 0; ; t += step)
        {
            Vector2 samplePoint = center + dirNormalized * t;
            if (samplePoint.x < 0 || samplePoint.x >= width || samplePoint.y < 0 || samplePoint.y >= height) break;

            int x = Mathf.RoundToInt(samplePoint.x);
            int y = Mathf.RoundToInt(samplePoint.y);
            if (noiseValue[x, y] <= 0f) continue; // 没有瓦片

            Vector2 toTile = new Vector2(x, y) - center;
            // 方向投影，判断是否在指定方向上
            float projection = Vector2.Dot(toTile.normalized, dirNormalized);
            if (projection < 0.99f) continue; // 方向偏差太大，忽略

            float distance = toTile.magnitude;
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestTile = new Vector2(x, y);
            }
        }

        Vector2 farthest = (farthestTile - center) / center;// 归一化
        return farthest;
    }
    private List<Vector2> GetAllPreferPositions()
    {
        List<Vector2> result = new List<Vector2>();

        foreach (EMapPreferencePosition pos in Enum.GetValues(typeof(EMapPreferencePosition)))
        {
            if (pos == EMapPreferencePosition.Random) continue;
            if ((preferPos & pos) != 0)
            {
                // 除了基础地面，其余地皮层都以(0,0)到偏好点的中间为中心，来生成椭圆形状的地面分布
                if (preferPos == EMapPreferencePosition.Random) return result;
                else
                {
                    Vector2 groundCenter = GetGroundCenter(pos);
                    result.Add(groundCenter);
                }
            }
        }

        return result;
    }
    private Vector2 GetPreferPosNormalized(EMapPreferencePosition pos)
    {
        switch (pos)
        {
            case EMapPreferencePosition.TopLeft: return new Vector2(-1f, 1f);
            case EMapPreferencePosition.TopCenter: return new Vector2(0f, 1f);
            case EMapPreferencePosition.TopRight: return new Vector2(1f, 1f);

            case EMapPreferencePosition.MiddleLeft: return new Vector2(-1f, 0f);
            case EMapPreferencePosition.Center: return new Vector2(0f, 0f);
            case EMapPreferencePosition.MiddleRight: return new Vector2(1f, 0f);

            case EMapPreferencePosition.BottomLeft: return new Vector2(-1f, -1f);
            case EMapPreferencePosition.BottomCenter: return new Vector2(0f, -1f);
            case EMapPreferencePosition.BottomRight: return new Vector2(1f, -1f);

            default: return Vector2.zero;
        }
    }
    private Vector2 GetGroundCenter(EMapPreferencePosition position)
    {
        Vector2 p = GetPreferPosNormalized(position);
        bool isBaseGround = generatorData.layer == EMapLayer.BaseGround;
        MapGenerator baseGround = isBaseGround ? this : MapManager.Instance.GetMapByLayer(EMapLayer.BaseGround);

        if (position == EMapPreferencePosition.Center) return p;
        else return 0.5f * baseGround.FindFarthestTile(p);
    }
    private float GetEllipseMask(int x, int y)
    {
        if (preferPos == EMapPreferencePosition.Random) return 1;

        float finalMask = 0;
        foreach (Vector2 p in allPreferPos)
        {
            // 中心坐标
            float cx = (p.x + shapeParams.centerX) * width;
            float cy = (p.y + shapeParams.centerY) * height;

            // 椭圆 dx/dy
            float dx = x - cx;
            float dy = y - cy;

            // 扭曲
            float nx = x / (float)width; // 用归一化去做 Perlin
            float ny = y / (float)height;
            float offsetX = (Mathf.PerlinNoise(nx * shapeParams.warpScale + shapeParams.seedX, ny * shapeParams.warpScale + shapeParams.seedY)) * shapeParams.warp;
            float offsetY = (Mathf.PerlinNoise(nx * shapeParams.warpScale + shapeParams.seedX + 100, ny * shapeParams.warpScale + shapeParams.seedY + 100) - 0.5f) * shapeParams.warp;

            dx += offsetX * width;
            dy += offsetY * height;

            float a = shapeParams.a * width * 0.5f;
            float b = shapeParams.b * height * 0.5f;
            // 椭圆公式（像素单位）
            float value = (dx * dx) / (a * a) + (dy * dy) / (b * b);
            // mask
            var mask = 1f - Mathf.Clamp01(value);
            mask = Mathf.Pow(mask, 2f); // 可以调节边缘平滑度

            finalMask += mask;
        }
        finalMask = Mathf.Clamp01(finalMask);
        return finalMask;
    }
    private void SetTileMap()
    {
        float randomOffset = (float)(random.NextDouble() * 2000 - 1000);
        float noiseMax = float.MinValue;
        float noiseMin = float.MaxValue;
        bool isRandom = preferPos == EMapPreferencePosition.Random;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float nx = isRandom ? x : x / (float)width;
                float ny = isRandom ? y : y / (float)height;

                float perlinNoise = Mathf.PerlinNoise(nx * lacunarity + randomOffset, ny * lacunarity + randomOffset);
                float mask = GetEllipseMask(x, y);
                float noise = perlinNoise * mask;

                if (noise < noiseMin) noiseMin = noise;
                if (noise > noiseMax) noiseMax = noise;

                noiseValue[x, y] = noise;
            }
        }
        float range = noiseMax - noiseMin;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = noiseValue[x, y];
                noiseValue[x, y] = (noise - noiseMin) / range;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isGround = IsGround(new Vector2Int(x, y));
                if (!isGround) continue;

                int posX = Mathf.CeilToInt(x - 0.5f * width);
                int posY = Mathf.CeilToInt(y - 0.5f * height);
                tilemap.SetTile(new Vector3Int(posX, posY), tileGround);
            }
        }
    }
    private void GenerateSpawn()
    {
        spawns.Clear();
        spawnBitMap = new BitArray(width * height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int index = new Vector2Int(x, y);

                float noise = noiseValue[x, y];
                bool isGround = IsGround(new Vector2Int(x, y));
                if (!isGround) continue;

                bool isSpawnIsGround = spawnGenerator.IsGround(x,y);
                if (!isSpawnIsGround) continue;

                bool isEdge = IsEdgeTileOrEmpty(index);
                if (isEdge) continue;

                float chance = random.Next(0, 1000) * 0.001f;
                foreach (var spawnConf in spawns.confs)
                {
                    (bool isGenPlant, var mapSpawnObj) = spawnGenerator.GenSpawnObj(noise, index, chance, spawnConf);
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
        mapQuadTree.UpdateVisibleNodes<MapSpawnObj>(viewRect, (_isVisible, _obj) => 
        {
            _obj.SetVisible(_isVisible);
        });
    }
    private Vector2 MapIndexToWorldPos(Vector2Int mapIndex)
    {
        // 将数组索引映射回世界坐标
        float x = mapIndex.x - 0.5f * width;
        float y = mapIndex.y - 0.5f * height;

        return new Vector2(x, y);
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
        if (generatorData.layer != EMapLayer.BaseGround)
        {
            Vector2 worldPos = MapIndexToWorldPos(index);
            var baseGround = MapManager.Instance.GetMapByLayer(EMapLayer.BaseGround);
            if (baseGround != null && baseGround.IsEmptyTile(worldPos)) return false;
        }

        int x = index.x;
        int y = index.y;
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        bool isGround = threshold.x < noiseValue[x, y] && noiseValue[x, y] <= threshold.y;
        return isGround;
    }
    private bool IsEdgeTileOrEmpty(Vector2Int index)
    {
        int x = index.x;
        int y = index.y;

        // 当前格子不是地面 → 空格
        bool isGround = IsGround(index);
        if (!isGround) return true;

        // 遍历周围 8 个格子
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                // 跳过自己
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                // 超出边界 → 当作空格处理 → 是边缘
                if (nx < 0 || nx >= width || ny < 0 || ny >= height) return true;

                // 周围有一个格子不是地面 → 边缘
                if (!IsGround(new Vector2Int(nx, ny))) return true;
            }
        }

        // 周围都是地面 → 不是边缘，也不是空格
        return false;
    }
}
[Serializable]
public class MapSpawnGenData
{
    public Transform root;
    [MinMaxRangeSlider(0, 1)] public Vector2 threshold; //阈值
    public List<MapSpawnConfig> confs = new();
    public void Clear()
    {
        if (root != null) root.RemoveChildren();
    }
}
