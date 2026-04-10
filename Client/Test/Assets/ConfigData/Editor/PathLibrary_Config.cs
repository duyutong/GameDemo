using System;

/// <summary>
/// 配置文件地址
/// </summary>
[Serializable]
public class PathLibrary_Config
{
    /// <summary>
    /// 存放配置表的文件目录
    /// </summary>
    public string excelPath;
    /// <summary>
    /// 存放json的文件目录
    /// </summary>
    public string jsonPath;
    /// <summary>
    /// 存放二进制的文件目录
    /// </summary>
    public string binaryPath;
    /// <summary>
    /// 存放配置类的文件目录
    /// </summary>
    public string csharpPath;
    /// <summary>
    /// 配置加载类的地址（全名）
    /// </summary>
    public string loaderPath;
}
