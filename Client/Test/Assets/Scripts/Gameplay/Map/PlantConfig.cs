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
    public void SetGroundSize(int width, int height) { this.width = width; this.height = height; }
    public bool GenPlant(float groundNoise, Vector2 pos, float chance)
    {
        float plantNoise = Mathf.InverseLerp(0, groundThreshold, groundNoise);
        int layout = height - Mathf.RoundToInt(Mathf.InverseLerp(-0.5f * height, 0.5f * height, pos.y) * height);
        layout *= 10;
        if (min <= plantNoise && plantNoise <= max)
        {
            if (probability.x <= chance && chance < probability.y)
            {
                GameObject plant = Instantiate(prefab);
                plant.transform.SetParent(plantRoot);
                plant.transform.Reset();
                plant.transform.localPosition = pos + offset;

                SpriteRenderer sr = plant.GetComponent<SpriteRenderer>();
                sr.sortingOrder = layout;

                return true;
            }
        }
        return false;
    }
    [MenuItem("GameObject/CreatePlantConfig", false, 0)]
    public static void CreatePlantConfig()
    {
        foreach (var selected in Selection.gameObjects)
        {
            if (selected == null) return;

            PlantConfig config = CreateInstance<PlantConfig>();
            config.plantName = selected.name;
            config.prefab = selected.GetPrefabDefinition().GameObject();

            string filePath = $"Assets/GameAssets/PerlinNoiseMap/PlantConfig/{selected.name}.asset";
            AssetDatabase.CreateAsset(config, filePath);
        }
    }
}
