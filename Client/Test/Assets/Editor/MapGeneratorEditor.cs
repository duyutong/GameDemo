using UnityEditor;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor:Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate")) 
        {
            ((MapGenerator)target).GenerateMap();
        }
        if (GUILayout.Button("Clear"))
        {
            ((MapGenerator)target).ClearTileMap();
        }
        if (GUILayout.Button("ShowPlant"))
        {
            ((MapGenerator)target).ShowAllPlant();
        }
    }
}
