using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfigDataJson<T> : IConfigDataHandler<T> where T : BaseConfig, new()
{
    private string tablePath;
    private Dictionary<int, T> configTable;
    public ConfigDataJson() { }
    public void SetTablePath(string path) => tablePath = path;
    public T GetConfigData(int id)
    {
        configTable ??= LoadConfigTable();
        if (!configTable.ContainsKey(id)) return null;
        return configTable[id];
    }
    public List<T> GetConfigData(Func<T, bool> select, int count = 1)
    {
        configTable ??= LoadConfigTable();
        return configTable.Values.Where(v => select(v)).Take(count).ToList();
    }
    public Dictionary<int, T> LoadConfigTable()
    {
        Dictionary<int, T> result = new();
        string json = LoadTableData(tablePath);
        JArray array = JArray.Parse(json);
        for (int i = 0; i < array.Count; i++)
        {
            JObject item = array[i] as JObject;
            Dictionary<string, object> pairs = item.ToObject<Dictionary<string, object>>();

            T confItem = new();
            confItem.Initialize(pairs);

            result.Add(confItem.id, confItem);
        }
        return result;
    }
    private string LoadTableData(string tablePathOrKey) 
    {
#if UNITY_EDITOR
        // 编辑器阶段：直接读取数据
        return File.ReadAllText(tablePathOrKey);
#else
        // 运行时：走 Addressables
        TextAsset txt = Addressables.LoadAssetAsync<TextAsset>(tablePathOrKey).WaitForCompletion();
        return txt != null ? txt.text : null;
#endif
    }
}

