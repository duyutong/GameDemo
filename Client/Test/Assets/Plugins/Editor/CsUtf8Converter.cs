using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class CsUtf8BomConverter : EditorWindow
{
    private string folderPath = "Assets"; // 默认扫描目录为 Assets 文件夹

    [MenuItem("Tools/Convert .cs to UTF-8 BOM")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CsUtf8BomConverter), false, "UTF-8 BOM 转换工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量转换 .cs 文件为 UTF-8（带 BOM）编码", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        folderPath = EditorGUILayout.TextField("扫描目录", folderPath);

        if (GUILayout.Button("选择目录"))
        {
            string selected = EditorUtility.OpenFolderPanel("选择扫描目录", folderPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                folderPath = selected;
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("开始转换"))
        {
            if (Directory.Exists(folderPath))
            {
                ConvertAllCsFiles(folderPath);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("完成", "转换完成，如果没有打印请再运行一次", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "目录不存在", "确定");
            }
        }
    }

    private void ConvertAllCsFiles(string rootPath)
    {
        string[] files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);
        int count = 0;

        foreach (var file in files)
        {
            try
            {
                string content = File.ReadAllText(file, DetectEncoding(file));
                // 写入 UTF-8 并添加 BOM
                File.WriteAllText(file, content, new UTF8Encoding(true));
                count++;
                Debug.Log($"已转换: {file}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"转换失败: {file} 原因: {ex.Message}");
            }
        }

        Debug.Log($"转换完成，共处理 {count} 个文件");
    }

    private Encoding DetectEncoding(string filePath)
    {
        using (var reader = new StreamReader(filePath, true))
        {
            if (reader.Peek() >= 0)
            {
                reader.Read();
            }
            return reader.CurrentEncoding;
        }
    }
}
