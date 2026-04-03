using UnityEngine;

public class MapSpawnGenerator
{
    private MapSpawnGenData spawnGenData;
    private int seed;
    private System.Random random;

    private Vector2 offset = 0.5f * Vector2.one;
    private int width;
    private int height;
    private Vector2 grpundThreshold;
    private bool[,] noiseValue;
    public void SetGroundThreshold(float min, float max) => grpundThreshold = new Vector2(min, max);
    public void SetSeed(int seed) { this.seed = seed; GenerateNoise(); }
    public void SetSpawnData(MapSpawnGenData data) => spawnGenData = data;
    public void SetMapSize(int width, int height) { this.width = width; this.height = height; }
    private void GenerateNoise()
    {
        noiseValue = new bool[width, height];
        random = new(seed);

        Vector2 threshold = spawnGenData.threshold;
        float randomOffset = (float)(random.NextDouble() * 2000 - 1000);
        float lacunarity = 0.1f;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * lacunarity + randomOffset, y * lacunarity + randomOffset);
                bool isGround = threshold.x < noise && noise <= threshold.y;
                noiseValue[x, y] = isGround;
            }
        }
    }
    public bool IsGround(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;
        return noiseValue[x, y];
    }
    public (bool, MapSpawnObj) GenSpawnObj(float groundNoise, Vector2Int index, float chance, MapSpawnConfig config)
    {
        float posX = index.x - 0.5f * width;
        float posY = index.y - 0.5f * height;
        Vector2 pos = new(posX, posY);

        float spawntNoise = Mathf.InverseLerp(grpundThreshold.x, grpundThreshold.y, groundNoise);
        int layout = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, pos.y) * height);
        layout *= 10; pos += offset;

        Transform spawnRoot = spawnGenData.root;
        if (config.min < spawntNoise && spawntNoise <= config.max)
        {
            if (config.probability.x <= chance && chance < config.probability.y)
            {
                GameObject plant = GameObject.Instantiate(config.prefab);
                plant.transform.SetParent(spawnRoot);
                plant.transform.Reset();
                plant.transform.localPosition = pos;

                MapSpawnObj mapObstacleObj = plant.GetComponent<MapSpawnObj>();
                mapObstacleObj.SetIndexOnMap(index, spawntNoise);
                mapObstacleObj.SetVisible(false);
                mapObstacleObj.SetCameraOcclusionHit(false, true);

                int siblingIndex = plant.transform.GetSiblingIndex();
                int remainder = siblingIndex % 4;
                float ratio = 1;
                if (remainder < 2) ratio = 0.9f; else ratio = 1.2f;
                plant.transform.localScale = Vector3.one * ratio;

                SpriteRenderer sr = mapObstacleObj.spriteRenderer;
                sr.flipX = siblingIndex % 3 == 1;
                sr.sortingOrder = layout;

                return (true, mapObstacleObj);
            }
        }
        return (false, null);
    }
}
