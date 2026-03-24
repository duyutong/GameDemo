using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class NetworkPathConfig
{
    private static string libraryPath;
    private static string clientRoot;
    private static string clientNetworkRoot = "Network/";
    private static string clientApiPath = "API/";
    private static string clientEnumDefinitionsPath = "Scripts/EnumDefinitions.cs";

    private static string modelPath = "Models/";
    private static string commModelPath = "Models/Common/";

    private static string serverRoot = "F:\\Study\\ServerStu\\Server-Client\\FlexiServer";
    private static string servicePath = "Services/";
    private static string httpEndPointsPath = "Transport/Http/";
    private static string serverCorePath = "Core/";

    private static PathLibrary_Network pathLibrary = null;
    private static void InitLibraryPath()
    {
        clientRoot = Directory.GetParent(Application.dataPath).FullName;
        DirectoryInfo dir = new DirectoryInfo(clientRoot);
        string projectRoot = dir.Parent.Parent.FullName;
        string serverRoot = Path.Combine(projectRoot, "Server");

        libraryPath = Path.Combine(serverRoot, "PathLibrary_Network.json");
    }
    private static void InitPathLibrary()
    {
        if (string.IsNullOrEmpty(libraryPath)) InitLibraryPath();

        string json = File.ReadAllText(libraryPath);
        pathLibrary = JsonConvert.DeserializeObject<PathLibrary_Network>(json);
        DirectoryInfo dir = new DirectoryInfo(libraryPath);

        serverRoot = Path.Combine(dir.Parent.FullName, pathLibrary.serverRoot);
        clientNetworkRoot = pathLibrary.clientNetworkRoot;
        commModelPath = pathLibrary.commModelPath;
        servicePath = pathLibrary.servicePath;
        httpEndPointsPath = pathLibrary.httpEndPointsPath;
        serverCorePath = pathLibrary.serverCorePath;
        clientEnumDefinitionsPath = pathLibrary.clientEnumDefinitionsPath;
        clientApiPath = pathLibrary.clientApiPath;
        modelPath = pathLibrary.modelPath;
    }
    public static string GetClientApiFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientApiPath);
    }
    public static string GetClientModelFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientNetworkRoot, modelPath);
    }
    public static string GetClientCommModelFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientNetworkRoot, commModelPath);
    }
    public static string GetServerRootFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return serverRoot;
    }
    public static string GetServerHttpEndPointsFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, httpEndPointsPath);
    }
    public static string GetServerServiceFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, servicePath);
    }
    public static string GetServerModelFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, modelPath);
    }
    public static string GetServerCommModelFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, commModelPath);
    }
    public static string GetServerProgramFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, "Program.cs");
    }
    public static string GetClientApiManagerFullPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientNetworkRoot, "Api/ApiManager.cs");
    }
    public static string GetClientEnumDefinitionsPath() 
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientEnumDefinitionsPath);
    }
    public static string GetServerEnumDefinitionsPath() 
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, serverCorePath);
    }
    public static string GetClientNetworkEventPathsPath() 
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(clientRoot, clientNetworkRoot, "NetworkEventPaths.cs");
    }
    public static string GetServerNetworkEventPathsPath()
    {
        if (pathLibrary == null) InitPathLibrary();
        return Path.Combine(serverRoot, serverCorePath, "NetworkEventPaths.cs");
    }
}
