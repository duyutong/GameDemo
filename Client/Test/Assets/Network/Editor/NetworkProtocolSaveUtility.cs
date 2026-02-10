using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkProtocolSaveUtility
{
    public static void GenServer_Model(Dictionary<string, List<NetworkProtocolBlockData>> saveData)
    {
        string savePath = NetworkPathConfig.GetServerCommModelFullPath();
        foreach (KeyValuePair<string, List<NetworkProtocolBlockData>> keyValuePair in saveData)
        {
            EditorUtilityExtensions.ToCamelAndPascal(keyValuePair.Key, out _, out string modelName_uc);
            List<NetworkProtocolBlockData> blocks = keyValuePair.Value;
            if (string.IsNullOrEmpty(modelName_uc)) continue;
            if (blocks == null || blocks.Count == 0) continue;
            GenServer_SingleModel(modelName_uc, savePath, blocks, true);
        }
    }
    private static void GenServer_SingleModel(string modelName, string savePath, List<NetworkProtocolBlockData> blocks, bool isComm = false)
    {
        string tempStr = CSTemplate_Network.ModelStr;
        tempStr = tempStr.Replace("#ModelName#", modelName);
        tempStr = tempStr.Replace("#NamespaceStr#", isComm ? CSTemplate_Network.NamespaceStr_ModelsCommon : CSTemplate_Network.NamespaceStr_Models);

        string variableStr = "";
        foreach (NetworkProtocolBlockData block in blocks)
        {
            string str = block.isEnumerable ? CSTemplate_Network.ModelVariableListStr : CSTemplate_Network.ModelVariableStr;
            EditorUtilityExtensions.ToCamelAndPascal(block.variableName, out _, out string variableName_uc);
            str = str.Replace("#TypeName#", block.typeName);
            str = str.Replace("#VariableName#", variableName_uc);
            variableStr += str;
        }
        tempStr = tempStr.Replace("#Variable#", variableStr);

        //保存文件
        string csSavePath = Path.Combine(savePath, $"{modelName}.cs");
        SaveToFile(csSavePath, tempStr);
    }
    public static void GenServer_HttpProtocol(NetworkProtocolSaveData saveData)
    {
        if (string.IsNullOrEmpty(saveData.protoclName)) return;
        string savePath = NetworkPathConfig.GetServerModelFullPath();
        GenServer_HttpEndpoints(saveData.protoclName, saveData.eventData);
        GenServer_Service(saveData.protoclName, saveData.eventData);
        foreach (NetworkProtocolEventSaveData eventData in saveData.eventData)
        {
            GenServer_SingleModel($"{GetFuncNameFromPattern(eventData.pattern)}Request", savePath, eventData.request);
            GenServer_SingleModel($"{GetFuncNameFromPattern(eventData.pattern)}Response", savePath, eventData.response);
        }
    }
    private static void GenServer_HttpEndpoints(string protoclName, List<NetworkProtocolEventSaveData> eventData)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string tempStr = CSTemplate_Network.HttpEndpointsStr;
        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        tempStr = tempStr.Replace("#ProtocolName_UC#", protocol_uc);
        tempStr = tempStr.Replace("#ProtocolName_LC#", protocol_lc);

        string MapPostStr = "";
        foreach (NetworkProtocolEventSaveData eventSaveData in eventData)
        {
            string str = CSTemplate_Network.MapPostStr;
            string funcName = GetFuncNameFromPattern(eventSaveData.pattern);
            str = str.Replace("#Pattern#", eventSaveData.pattern);
            str = str.Replace("#ProtocolName_LC#", protocol_lc);
            str = str.Replace("#ProtocolName_UC#", protocol_uc);
            str = str.Replace("#Func#", funcName);
            MapPostStr += str;
        }
        tempStr = tempStr.Replace("#MapPostStr#", MapPostStr);
        //保存文件
        string savePath = NetworkPathConfig.GetServerHttpEndPointsFullPath();
        string endpointsName = $"{protocol_uc}Endpoints";
        string csSavePath = Path.Combine(savePath, $"{endpointsName}.cs");
        SaveToFile(csSavePath, tempStr);
    }
    private static void GenServer_Service(string protoclName, List<NetworkProtocolEventSaveData> eventData)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out _, out protocol_uc);
        string savePath = NetworkPathConfig.GetServerServiceFullPath();
        string serverName = $"{protocol_uc}Service";
        string csSavePath = Path.Combine(savePath, $"{serverName}.cs");

        string exitStr = EditorUtilityExtensions.ReadFileIfExists(csSavePath);
        exitStr = string.IsNullOrEmpty(exitStr)? CSTemplate_Network.HttpServiceStr:exitStr;
        
        foreach (NetworkProtocolEventSaveData eventSaveData in eventData)
        {
            string funcName = GetFuncNameFromPattern(eventSaveData.pattern);
            if (exitStr.Contains($"public async Task<{funcName}Response> {funcName}(HttpMessage<{funcName}Request> msg)")) continue;

            string str = CSTemplate_Network.HttpFuncStr;
            str = str.Replace("#Func#", funcName);
            str += "\n        #endregion HttpFuncStr";
            exitStr = exitStr.Replace("#endregion HttpFuncStr", str);
        }
        exitStr = exitStr.Replace("#ProtocolName_UC#", protocol_uc);
        //保存文件
        SaveToFile(csSavePath, exitStr);
    }
    public static void GenClient_Model(Dictionary<string, List<NetworkProtocolBlockData>> saveData)
    {
        string savePath = NetworkPathConfig.GetClientCommModelFullPath();
        foreach (KeyValuePair<string, List<NetworkProtocolBlockData>> keyValuePair in saveData)
        {
            string modelName;
            EditorUtilityExtensions.ToCamelAndPascal(keyValuePair.Key, out _, out modelName);
            List<NetworkProtocolBlockData> blocks = keyValuePair.Value;
            if (string.IsNullOrEmpty(modelName)) continue;
            if (blocks == null || blocks.Count == 0) continue;
            GenClient_SingleModel(modelName, savePath, blocks, true);
        }
    }
    private static void GenClient_SingleModel(string modelName, string savePath, List<NetworkProtocolBlockData> blocks, bool isComm = false)
    {
        bool isStar = modelName.StartsWith("*");
        modelName = isStar ? modelName.Substring(1) : modelName;

        string tempStr = CSTemplate_Network.ClientModelStr;
        tempStr = tempStr.Replace("#ModelName#", modelName);
        tempStr = tempStr.Replace("#NamespaceStr#", isComm ? CSTemplate_Network.NamespaceStr_ModelsCommon : CSTemplate_Network.NamespaceStr_Models);
        string variableStr = "";
        foreach (NetworkProtocolBlockData block in blocks)
        {
            string str = block.isEnumerable ? CSTemplate_Network.ModelVariableListStr : CSTemplate_Network.ModelVariableStr;
            EditorUtilityExtensions.ToCamelAndPascal(block.variableName, out _, out string variableName_uc);
            str = str.Replace("#TypeName#", block.typeName);
            str = str.Replace("#VariableName#", variableName_uc);
            variableStr += str;
        }
        tempStr = tempStr.Replace("#Variable#", variableStr);
        //保存文件
        string csSavePath = Path.Combine(savePath, $"{modelName}.cs");
        SaveToFile(csSavePath, tempStr);
    }
    public static void GenClient_HttpProtocol(NetworkProtocolSaveData saveData)
    {
        if (string.IsNullOrEmpty(saveData.protoclName)) return;

        GenClient_APi(saveData.protoclName, saveData.eventData);

        string savePath = NetworkPathConfig.GetClientModelFullPath();
        foreach (NetworkProtocolEventSaveData eventData in saveData.eventData)
        {
            GenClient_SingleModel($"{GetFuncNameFromPattern(eventData.pattern)}Request", savePath, eventData.request);
            GenClient_SingleModel($"{GetFuncNameFromPattern(eventData.pattern)}Response", savePath, eventData.response);
        }
    }
    private static void GenClient_APi(string protoclName, List<NetworkProtocolEventSaveData> eventData)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        string savePath = NetworkPathConfig.GetClientApiFullPath();
        string csSavePath = Path.Combine(savePath, $"{protocol_uc}Api.cs");
        string exitStr = EditorUtilityExtensions.ReadFileIfExists(csSavePath);
        exitStr = string.IsNullOrEmpty(exitStr) ? CSTemplate_Network.HttpApiStr : exitStr;

        foreach (NetworkProtocolEventSaveData eventSaveData in eventData)
        {
            string funcName = GetFuncNameFromPattern(eventSaveData.pattern);
            if (exitStr.Contains($"public async void {funcName}")) continue;

            string str = CSTemplate_Network.HttpApiFuncStr;
            str = str.Replace("#Pattern#", eventSaveData.pattern);
            str = str.Replace("#ProtocolName_UC#", protocol_uc);
            str = str.Replace("#Func#", funcName);
            str += "\n         #endregion HttpFuncStr";
            exitStr = exitStr.Replace("#endregion HttpFuncStr", str);
        }
        exitStr = exitStr.Replace("#ProtocolName_UC#", protocol_uc);
        exitStr = exitStr.Replace("#ProtocolName_LC#", protocol_lc);
        SaveToFile(csSavePath, exitStr);
    }
    public static void GenClient_UdpMessageApi(string protoclName) 
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        string pattern = $"/{protocol_lc}";

        string savePath = NetworkPathConfig.GetClientApiFullPath();
        string csSavePath = Path.Combine(savePath, $"{protocol_uc}Api.cs");
        string tempStr = CSTemplate_Network.UdpMessageApiStr;
        tempStr = tempStr.Replace("#ProtocolName_UC#", protocol_uc);
        tempStr = tempStr.Replace("#ProtocolName_LC#", protocol_lc);
        tempStr = tempStr.Replace("#Pattern#", pattern);
        //保存文件
        SaveToFile(csSavePath, tempStr);
    }
    public static void GenClient_WebSocketMessageApi(string protoclName)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        string pattern = $"/{protocol_lc}";

        string savePath = NetworkPathConfig.GetClientApiFullPath();
        string csSavePath = Path.Combine(savePath, $"{protocol_uc}Api.cs");
        string tempStr = CSTemplate_Network.WebSocketMessageApiStr;
        tempStr = tempStr.Replace("#ProtocolName_UC#", protocol_uc);
        tempStr = tempStr.Replace("#ProtocolName_LC#", protocol_lc);
        tempStr = tempStr.Replace("#Pattern#", pattern);
        //保存文件
        SaveToFile(csSavePath, tempStr);
    }
    public static void GenServer_SocketHandler(string protoclName, NetworkProtocolSaveData saveData)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        string pattern = $"/{protocol_lc}";
        string pattern_uc;
        EditorUtilityExtensions.ToCamelAndPascal(pattern, out _, out pattern_uc);

        string savePath = NetworkPathConfig.GetServerServiceFullPath();
        string csSavePath = Path.Combine(savePath, $"{protocol_uc}Service.cs");
        string exitStr = EditorUtilityExtensions.ReadFileIfExists(csSavePath);
        exitStr = string.IsNullOrEmpty(exitStr)? CSTemplate_Network.SocketHandlerStr:exitStr;
        
        foreach (NetworkProtocolEventSaveData eventData in saveData.eventData)
        {
            string path = eventData.pattern;
            string funcName = GetFuncNameFromPattern(path);
            if (exitStr.Contains($"{funcName}Handle(ClientId, recievMsg.Path, Msg);")) continue;

            string str1 = CSTemplate_Network.SwitchHandleStr;
            str1 = str1.Replace("#Path#", path);
            str1 = str1.Replace("#Pattern_UC#", pattern_uc);
            str1 = str1.Replace("#Func#", funcName);
            str1 += "\n                 #endregion Switch_Handle";
            exitStr = exitStr.Replace("#endregion Switch_Handle", str1);

            string str2 = CSTemplate_Network.FunctionHandleStr;
            str2 = str2.Replace("#Func#", funcName);
            str2 += "\n        #endregion Function_Handle";
            exitStr = exitStr.Replace("#endregion Function_Handle", str2);
        }

        exitStr = exitStr.Replace("#ProtocolName_UC#", protocol_uc);
        exitStr = exitStr.Replace("#ProtocolName_LC#", protocol_lc);
        exitStr = exitStr.Replace("#Pattern#", pattern);
        //保存文件
        SaveToFile(csSavePath, exitStr);
    }
    public static void RefreshApiManager_Socket(string protoclName)
    {
        if (string.IsNullOrEmpty(protoclName)) return;

        string protocol_uc, protocol_lc;
        EditorUtilityExtensions.ToCamelAndPascal(protoclName, out protocol_lc, out protocol_uc);
        string csSavePath = NetworkPathConfig.GetClientApiManagerFullPath();
        string exitStr = EditorUtilityExtensions.ReadFileIfExists(csSavePath);
        // 注入WebSocket协议的API映射
        string wsApiMapping = CSTemplate_Network.SocketApiMappingStr;
        wsApiMapping = wsApiMapping.Replace("#ProtocolName_UC#", protocol_uc);
        wsApiMapping = wsApiMapping.Replace("#ProtocolName_LC#", protocol_lc);
        wsApiMapping = wsApiMapping.Replace("#Pattern#", $"/{protocol_lc}");
        if (!exitStr.Contains(wsApiMapping))
        {
            wsApiMapping += "\n#endregion patternToType";
            exitStr = exitStr.Replace("#endregion patternToType", wsApiMapping);
        }
        //保存文件
        SaveToFile(csSavePath, exitStr);
    }
    public static void RefreshNetworkEventPaths_Socket(string pattern,NetworkProtocolSaveData saveData) 
    {
        string csSavePath_client = NetworkPathConfig.GetClientNetworkEventPathsPath();
        string exitStr_client = EditorUtilityExtensions.ReadFileIfExists(csSavePath_client);

        string csSavePath_server = NetworkPathConfig.GetServerNetworkEventPathsPath();
        string exitStr_server = EditorUtilityExtensions.ReadFileIfExists(csSavePath_server);

        string pattern_uc;
        EditorUtilityExtensions.ToCamelAndPascal(pattern, out _, out pattern_uc);
        foreach (NetworkProtocolEventSaveData eventData in saveData.eventData)
        {
            string networkEventPath = CSTemplate_Network.NetworkEventPathStr;
            string path = eventData.pattern;
            string funcName = GetFuncNameFromPattern(path);

            networkEventPath = networkEventPath.Replace("#Pattern_UC#", pattern_uc);
            networkEventPath = networkEventPath.Replace("#Func#", funcName);
            networkEventPath = networkEventPath.Replace("#Path#",path);

            if (!exitStr_client.Contains(networkEventPath)) 
            {
                string newPath = networkEventPath + "\n        #endregion NetworkEventPaths";
                exitStr_client = exitStr_client.Replace("#endregion NetworkEventPaths", newPath);
            }
            if (!exitStr_server.Contains(networkEventPath)) 
            {
                string newPath = networkEventPath + "\n        #endregion NetworkEventPaths";
                exitStr_server = exitStr_server.Replace("#endregion NetworkEventPaths", newPath);
            }
        }
        SaveToFile(csSavePath_client, exitStr_client);
        SaveToFile(csSavePath_server, exitStr_server);
    }
    public static void CopyEnumDefinitionsToServer()
    {
        string sourceFilePath = NetworkPathConfig.GetClientEnumDefinitionsPath();
        string destinationPath = NetworkPathConfig.GetServerEnumDefinitionsPath();
        EditorUtilityExtensions.CopyFile(sourceFilePath, destinationPath);
    }
    public static void SaveToFile(string savePath, string fileStr)
    {
        FileInfo saveInfo = new FileInfo(savePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(fileStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();
    }
    /// <summary>
    /// 将以 '/' 分隔的字符串转换为大驼峰（PascalCase）。
    /// </summary>
    /// <param name="pattern">输入的字符串，使用 '/' 分隔。</param>
    public static string GetFuncNameFromPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return string.Empty;

        // 用 '/' 分割，去掉空段
        var parts = pattern.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return string.Empty;

        // 每段首字母大写，其余保持原样，然后拼接
        var result = string.Concat(parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
        return result;


    }
}
public class NetworkProtocolSaveData
{
    public string connectionType;
    public string protoclName;
    public List<NetworkProtocolEventSaveData> eventData;
}
public class NetworkProtocolEventSaveData
{
    public string pattern;
    public List<NetworkProtocolBlockData> request;
    public List<NetworkProtocolBlockData> response;
}
public class NetworkProtocolBlockData
{
    public string typeName;
    public string variableName;
    public bool isEnumerable;
}
public class NetworkModelSaveData
{
    public List<NetworkProtocolBlockData> modelBlocks;
}