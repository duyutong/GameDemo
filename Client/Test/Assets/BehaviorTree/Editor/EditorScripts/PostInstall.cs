using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public static class PostInstall
{
    static PostInstall()
    {
        // Դ�ļ��У������ AssetsContent
        string source = Path.Combine("Packages", "com.duyutong.uianimabehaviortree");
        // Ŀ���ļ��У���Ŀ Assets ��
        string dest = Path.Combine("Assets", "BehaviorTree");

        if (Directory.Exists(source) && !Directory.Exists(dest))
        {
            CopyDirectory(source, dest);
            AssetDatabase.Refresh();
            Debug.Log("UIAnimaBehaviorTree �Ѱ�װ�� Assets Ŀ¼�£�");
        }
    }

    static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
            CopyDirectory(dir, destSubDir);
        }
    }
}