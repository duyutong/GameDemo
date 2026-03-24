using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ConfigDataJson<T> : IConfigDataHandler<T> where T : BaseConfig, new()
{
    private string tablePath;
    private Dictionary<int, T> configTable;
    private readonly object _lock = new object();
    public ConfigDataJson() { }
    public void SetTablePath(string path) => tablePath = path;
    public T GetConfigData(int id)
    {
        if (configTable == null)
        {
            lock (_lock)
            { configTable ??= LoadConfigTable(); }
        }
        if (!configTable.ContainsKey(id)) return null;
        return configTable[id];
    }
    public List<T> GetConfigData(Func<T, bool> select, int count = 1)
    {
        if (configTable == null)
        {
            lock (_lock)
            { configTable ??= LoadConfigTable(); }
        }
        return configTable.Values.Where(v => select(v)).Take(count).ToList();
    }
    public Dictionary<int, T> LoadConfigTable()
    {
        Dictionary<int, T> result = new();
        string json = File.ReadAllText(tablePath);
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
}

