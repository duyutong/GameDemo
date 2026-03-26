using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantConfig", menuName = "Scriptable Objects/PlantConfig")]
public class PlantConfig : ScriptableObject
{
    public string plantName;
    public GameObject prefab;
    [Range(0, 1)] public float min = 0.1f;
    [Range(0, 1)] public float max = 0.5f;
    [MinMaxRangeSlider(0, 1)] public Vector2 probability = new Vector2(0.2f, 0.5f);

    private Vector2 offset = 0.5f * Vector2.one;
    private Transform plantRoot;
    private float groundThreshold;
    private int width;
    private int height;
    public void SetPlantRoot(Transform root) => plantRoot = root;
    public void SetGroundThreshold(float threshold) => groundThreshold = threshold;
    public void SetMapSize(int width, int height) { this.width = width; this.height = height; }
    public (bool, MapObstacleObj) GenPlant(float groundNoise, Vector2Int index, float chance)
    {
        float posX = index.x - 0.5f * width;
        float posY = index.y - 0.5f * height;
        Vector2 pos = new(posX, posY);

        float plantNoise = Mathf.InverseLerp(0, groundThreshold, groundNoise);
        int layout = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, pos.y) * height);
        layout *= 10; pos += offset;

        if (min <= plantNoise && plantNoise <= max)
        {
            if (probability.x <= chance && chance < probability.y)
            {
                GameObject plant = Instantiate(prefab);
                plant.transform.SetParent(plantRoot);
                plant.transform.Reset();
                plant.transform.localPosition = pos;

                MapObstacleObj mapObstacleObj = plant.GetComponent<MapObstacleObj>();
                mapObstacleObj.SetIndexOnMap(index);
                mapObstacleObj.SetVisible(false,true);

                SpriteRenderer sr = mapObstacleObj.spriteRenderer;
                sr.flipX = plant.transform.GetSiblingIndex() % 3 == 1;
                sr.sortingOrder = layout;

                return (true, mapObstacleObj);
            }
        }
        return (false,null);
    }

    #region 临时编辑器工具

    [MenuItem("GameObject/UpdatePlantPrefab", false, 0)]
    public static void UpdatePlantPrefab() 
    {
        foreach (var selected in Selection.gameObjects) 
        {
            if (selected == null) continue;
            if (selected.transform.childCount == 0 )continue;

            MapObstacleObj mapObstacleObj = selected.GetOrAddComponent<MapObstacleObj>();
            mapObstacleObj.spriteRenderer = selected.transform.GetChild(0).GetComponent<SpriteRenderer>();
            mapObstacleObj.boxCollider = selected.transform.GetChild(0).GetComponent<BoxCollider2D>();
        }
    }
    [MenuItem("GameObject/CreatePlantPrefab", false, 0)]
    public static void CreatePlantPrefab()
    {
        string layerName = "Environment";
        int layer = LayerMask.NameToLayer(layerName);

        foreach (var selected in Selection.gameObjects)
        {
            if (selected == null) continue;


            if (selected.transform.parent != null && selected.transform.parent.name == selected.name + "_Root")
                continue;

            BoxCollider2D collider2D = selected.GetOrAddComponent<BoxCollider2D>();
            collider2D.isTrigger = true;

            selected.layer = layer;

            GameObject parent = new GameObject(selected.name + "_Root");
            parent.transform.position = selected.transform.position;
            parent.transform.rotation = selected.transform.rotation;

            selected.transform.SetParent(parent.transform);

            parent.layer = layer;

            MapObstacleObj mapObstacleObj = parent.GetOrAddComponent<MapObstacleObj>();
            mapObstacleObj.spriteRenderer = selected.GetComponent<SpriteRenderer>();
            mapObstacleObj.boxCollider = collider2D;
        }
    }
    [MenuItem("GameObject/CreatePlantConfig", false, 0)]
    public static void CreatePlantConfig()
    {
        foreach (var selected in Selection.gameObjects)
        {
            if (selected == null) continue;

            PlantConfig config = CreateInstance<PlantConfig>();
            config.plantName = selected.name;
            config.prefab = selected.GetPrefabDefinition().GameObject();

            string filePath = $"Assets/GameAssets/PerlinNoiseMap/PlantConfig/{selected.name}.asset";
            AssetDatabase.CreateAsset(config, filePath);
        }
    }
    #endregion
}
