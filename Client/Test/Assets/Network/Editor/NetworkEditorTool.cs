using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NetworkEditorTool
{
    [MenuItem("Network/Generator")]
    public static void OpenNetworkCodeGenerator()
    {
        NetworkCodeGenerator.OpenWindow();
    }
    [MenuItem("Network/OpenServerFolder")]
    public static void OpenServerFolder()
    {
        string path = NetworkPathConfig.GetServerProgramFullPath();
        EditorUtility.RevealInFinder(path);
    }
    [MenuItem("Network/StartServer")]
    public static void StartServer() 
    {
       string clientRoot = Directory.GetParent(Application.dataPath).FullName;
        DirectoryInfo dir = new DirectoryInfo(clientRoot);
        string projectRoot = dir.Parent.Parent.FullName;
        string serverRoot = Path.Combine(projectRoot, "Server");
        string batPath = Path.Combine(serverRoot, "StartServer_ForUnity.bat");

        try
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = batPath,
                UseShellExecute = true,   // 必须为 true 才能启动 .bat
                CreateNoWindow = false    // false = 会弹出黑窗口（便于看日志）
            };

            Process.Start(psi);
            UnityEngine.Debug.Log($"Start .bat: {batPath}");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to start bat: {e}");
        }
    }
}
