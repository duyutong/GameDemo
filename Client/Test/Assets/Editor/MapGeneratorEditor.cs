using UnityEditor;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapManager))]
public class MapGeneratorEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate")) 
        {
            ((MapManager)target).GenerateMap();
        }
        if (GUILayout.Button("Clear"))
        {
            ((MapManager)target).ClearTileMap();
        }
        if (GUILayout.Button("ShowPlant"))
        {
            ((MapManager)target).ShowAllPlant();
        }
    }
}
