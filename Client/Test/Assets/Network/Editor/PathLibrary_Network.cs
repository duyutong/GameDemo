using System;

/// <summary>
/// 网络相关路径配置
/// </summary>
[Serializable]
public class PathLibrary_Network
{
    /// <summary>
    /// 客户端网络根目录
    /// </summary>
    public string clientNetworkRoot;

    /// <summary>
    /// 客户端 API 目录
    /// </summary>
    public string clientApiPath;

    /// <summary>
    /// 客户端枚举定义文件路径
    /// </summary>
    public string clientEnumDefinitionsPath;

    /// <summary>
    /// 客户端 Model 目录
    /// </summary>
    public string modelPath;

    /// <summary>
    /// 客户端/服务器 公共 Model 目录
    /// </summary>
    public string commModelPath;

    /// <summary>
    /// 服务器根目录
    /// </summary>
    public string serverRoot;

    /// <summary>
    /// 服务器 Service 目录
    /// </summary>
    public string servicePath;

    /// <summary>
    /// 服务器/客户端 HTTP 端点目录
    /// </summary>
    public string httpEndPointsPath;

    /// <summary>
    /// 服务器核心模块目录
    /// </summary>
    public string serverCorePath;
}