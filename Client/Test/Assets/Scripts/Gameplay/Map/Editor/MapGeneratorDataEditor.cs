#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapGeneratorData))]
public class MapGeneratorDataEditor : Editor
{
    private bool showTileInfo = true;
    private bool showMapSize = true;
    private bool showSeed = true;

    public override void OnInspectorGUI()
    {
        var data = (MapGeneratorData)target;

        DrawDefaultInspector(); // 画剩下的

        // 瓦片信息
        showTileInfo = EditorGUILayout.Foldout(showTileInfo, "TileData");
        if (showTileInfo)
        {
            EditorGUI.indentLevel++;
            data.tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", data.tilemap, typeof(Tilemap), true);
            data.tileGround = (TileBase)EditorGUILayout.ObjectField("Tile Ground", data.tileGround, typeof(TileBase), false);
            EditorGUI.indentLevel--;
        }

        // 地图尺寸
        showMapSize = EditorGUILayout.Foldout(showMapSize, "MapSize");
        if (showMapSize)
        {
            EditorGUI.indentLevel++;
            data.width = EditorGUILayout.IntField("Width", data.width);
            data.height = EditorGUILayout.IntField("Height", data.height);
            EditorGUI.indentLevel--;
        }

        // 种子
        showSeed = EditorGUILayout.Foldout(showSeed, "Seed");
        if (showSeed)
        {
            EditorGUI.indentLevel++;
            data.seed = EditorGUILayout.IntField("Seed", data.seed);
            data.useNewSeed = EditorGUILayout.Toggle("Use New Seed", data.useNewSeed);
            EditorGUI.indentLevel--;
        }
        if (GUILayout.Button("GenerateMap")) MapManager.Instance.GenerateMap(data);
        if (GUILayout.Button("ClearMap")) MapManager.Instance.ClearMap(data.layer);
        if (GUILayout.Button("ShowAllSpawn")) MapManager.Instance.ShowAllSpawn(data.layer);
    }
}
#endif