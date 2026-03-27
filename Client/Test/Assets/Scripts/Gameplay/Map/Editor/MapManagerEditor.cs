using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        var manager = (MapManager)target;

        if (GUILayout.Button("IniMapGenerators")) manager.IniMapGenerators();
    }
}
