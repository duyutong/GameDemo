using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MapSpawnConfig", menuName = "Scriptable Objects/MapSpawnConfig")]
public class MapSpawnConfig : ScriptableObject
{
    public string spawnName;
    public GameObject prefab;
    [Range(0, 1)] public float min = 0.1f;
    [Range(0, 1)] public float max = 0.5f;
    [MinMaxRangeSlider(0, 1)] public Vector2 probability = new Vector2(0.2f, 0.5f);
}
