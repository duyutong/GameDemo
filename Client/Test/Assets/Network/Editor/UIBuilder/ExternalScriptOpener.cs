using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ExternalScriptOpener
{
    public static void OpenByFullPath(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            UnityEngine.Debug.LogError("脚本路径为空");
            return;
        }

        fullPath = fullPath.Replace("\\", "/");

        string dataPath = Application.dataPath.Replace("\\", "/");

        if (!fullPath.StartsWith(dataPath))
        {
            UnityEngine.Debug.LogError($"路径不在当前 Unity 工程内: {fullPath}");
            return;
        }

        // 转成 Assets 相对路径
        string assetPath = "Assets" + fullPath.Substring(dataPath.Length);

        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
        if (script == null)
        {
            UnityEngine.Debug.LogError($"未能加载脚本: {assetPath}");
            return;
        }

        AssetDatabase.OpenAsset(script);
    }
    public static void OpenWithChoice(string scriptFullPath, string slnFullPath)
    {
        if (!File.Exists(scriptFullPath))
        {
            UnityEngine.Debug.LogError($"脚本不存在: {scriptFullPath}");
            return;
        }

        if (!File.Exists(slnFullPath))
        {
            UnityEngine.Debug.LogError($"sln 不存在: {slnFullPath}");
            return;
        }

        bool openProject = EditorUtility.DisplayDialog(
             "打开脚本",
             "是否同时打开该脚本所属的工程？",
             "打开工程",
             "不必"
         );

        if (openProject) 
        {
            OpenSolution(slnFullPath);
            OpenScript(scriptFullPath);
        }
        else OpenScript(scriptFullPath);
    }

    private static void OpenScript(string scriptFullPath)
    {
        // 打开脚本
        Process.Start(new ProcessStartInfo
        {
            FileName = scriptFullPath,
            UseShellExecute = true
        });
    }

    private static void OpenSolution(string slnFullPath)
    {
        // 打开解决方案
        Process.Start(new ProcessStartInfo
        {
            FileName = slnFullPath,
            UseShellExecute = true
        });
    }
}
