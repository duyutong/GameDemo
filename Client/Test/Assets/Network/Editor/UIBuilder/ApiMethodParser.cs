using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;

public class ApiMethodParser
{
    public static List<NetworkProtocolEventSaveData> GetWebSocketProto(string protoName)
    {
        string path = NetworkPathConfig.GetClientNetworkEventPathsPath();
        string exitStr = EditorUtilityExtensions.ReadFileIfExists(path);

        var pattern = @"public\s+const\s+string\s+([A-Za-z_]+)_\w+\s*=\s*""([^""]+)"";";
        var matches = Regex.Matches(exitStr, pattern);

        List<NetworkProtocolEventSaveData> eventData = new List<NetworkProtocolEventSaveData>();
        foreach (Match match in matches)
        {
            var data1 = match.Groups[1].Value;
            if (data1 != protoName) continue;

            var data2 = match.Groups[2].Value;
            NetworkProtocolEventSaveData eventSaveData = new NetworkProtocolEventSaveData();
            eventSaveData.pattern = data2;
            eventData.Add(eventSaveData);
        }
        return eventData;
    }
    public static List<NetworkProtocolEventSaveData> GetHttpProto(string path)
    {
        string exitStr = EditorUtilityExtensions.ReadFileIfExists(path);

        var pattern = @"PostAsync<\s*([^,\s>]+)\s*,\s*([^>\s]+)\s*>\s*\(\s*""([^""]+)""";
        var matches = Regex.Matches(exitStr, pattern);

        List<NetworkProtocolEventSaveData> eventData = new List<NetworkProtocolEventSaveData>();
        foreach (Match match in matches)
        {
            var data1 = match.Groups[1].Value;
            var data2 = match.Groups[2].Value;
            var data3 = match.Groups[3].Value;

            NetworkProtocolEventSaveData eventSaveData = new NetworkProtocolEventSaveData();
            eventSaveData.pattern = data3;
            eventSaveData.request = GetBlockDatas(data1);
            eventSaveData.response = GetBlockDatas(data2);

            eventData.Add(eventSaveData);
        }
        return eventData;
    }
    private static List<NetworkProtocolBlockData> GetBlockDatas(string csName)
    {
        List<NetworkProtocolBlockData> blockDatas = new List<NetworkProtocolBlockData>();
        Type type = EditorUtilityExtensions.GetType($"Network.Models.{csName}");
        PropertyInfo[] properties = type.GetProperties();
        foreach (PropertyInfo property in properties)
        {
            Type propertyType;
            bool isEnumerable = EditorUtilityExtensions.TryGetListElementType(property, out propertyType);
            NetworkProtocolBlockData blockData = new NetworkProtocolBlockData();
            blockData.typeName = isEnumerable ? CommonValueTypes.GetShortNameByType(propertyType) : CommonValueTypes.GetShortNameByType(property.PropertyType);
            blockData.variableName = property.Name;
            blockData.isEnumerable = isEnumerable;
            blockDatas.Add(blockData);
        }
        return blockDatas;
    }
}
