using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NetworkEditorTool
{
    [MenuItem("Network/Generator")]
    public static void OpenNetworkCodeGenerator()
    {
        // OpenWithChoice the Network Code Generator window
        NetworkCodeGenerator.OpenWindow();
    }
    [MenuItem("Network/OpenServerFolder")]
    public static void OpenServerFolder()
    {
        string path = NetworkPathConfig.GetServerProgramFullPath();
        EditorUtility.RevealInFinder(path);
    }
}
