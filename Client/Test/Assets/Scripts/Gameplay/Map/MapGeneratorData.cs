using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneratorData : MonoBehaviour
{
    public EMapLayer layer;
    public MapSpawnGenData spawns;

    public EMapPreferencePosition position;
    [Range(0, 0.1f)] public float lacunarity;          //ÆµÂÊ
    [MinMaxRangeSlider(0, 1)] public Vector2 threshold; //ãÐÖµ

    [HideInInspector] public Tilemap tilemap;
    [HideInInspector] public TileBase tileGround;

    [HideInInspector] public int width;
    [HideInInspector] public int height;

    [HideInInspector] public int seed;
    [HideInInspector] public bool useNewSeed = false;
}
public enum EMapPreferencePosition
{
    Random,

    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    Center,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight
}