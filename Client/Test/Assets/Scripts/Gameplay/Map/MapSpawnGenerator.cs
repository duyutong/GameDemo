using UnityEngine;

public class MapSpawnGenerator
{
    private Vector2 offset = 0.5f * Vector2.one;
    private Transform spawnRoot;
    private Vector2 threshold;
    private int width;
    private int height;
    public void SetSpawnObjRoot(Transform root) => spawnRoot = root;
    public void SetGroundThreshold(Vector2 threshold) => this.threshold = threshold;
    public void SetMapSize(int width, int height) { this.width = width; this.height = height; }
    public (bool, MapSpawnObj) GenSpawnObj(float groundNoise, Vector2Int index, float chance, MapSpawnConfig config)
    {
        float posX = index.x - 0.5f * width;
        float posY = index.y - 0.5f * height;
        Vector2 pos = new(posX, posY);

        float spawntNoise = DistanceToCenter01(threshold.x, threshold.y, groundNoise);

        int layout = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, pos.y) * height);
        layout *= 10; pos += offset;

        if (config.min < spawntNoise && spawntNoise <= config.max)
        {
            if (config.probability.x <= chance && chance < config.probability.y)
            {
                GameObject plant = GameObject.Instantiate(config.prefab);
                plant.transform.SetParent(spawnRoot);
                plant.transform.Reset();
                plant.transform.localPosition = pos;

                MapSpawnObj mapObstacleObj = plant.GetComponent<MapSpawnObj>();
                mapObstacleObj.SetIndexOnMap(index);
                mapObstacleObj.SetVisible(false, true);

                SpriteRenderer sr = mapObstacleObj.spriteRenderer;
                sr.flipX = plant.transform.GetSiblingIndex() % 3 == 1;
                sr.sortingOrder = layout;

                return (true, mapObstacleObj);
            }
        }
        return (false, null);
    }
    // 中心的归一化距离
    private float DistanceToCenter01(float min, float max, float n)
    {
        if (Mathf.Approximately(min, max))return 0f;

        float center = (min + max) * 0.5f;
        float half = (max - min) * 0.5f;

        float t = Mathf.Abs(n - center) / half;
        t = Mathf.Clamp01(t);

        // 幂次曲线调整
        t = Mathf.Pow(t, 2);

        return t;
    }
}
