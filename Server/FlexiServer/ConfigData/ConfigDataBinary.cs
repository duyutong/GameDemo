using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ConfigDataBinary<T> : IConfigDataHandler<T> where T : BaseConfig
{
    private string tablePath;
    private Dictionary<int, T> configTable;
    private readonly object _lock = new object();
    private static MemoryStream tempStream = new MemoryStream(1024 * 32);
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
        Dictionary<int, T> result = new Dictionary<int, T>();
        string filePath = tablePath;
        tempStream = LoadFileAsMemoryStream(filePath);

        tempStream.Position = 0;
        List<T> table = ProtoBuf.Serializer.Deserialize<List<T>>(tempStream);
        foreach (T item in table)
        {
            item.id = GetIdOrDefault(item);
            int key = item.id;
            result[key] = item;
        }
        return result;
    }
    private static MemoryStream LoadFileAsMemoryStream(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        MemoryStream memoryStream = new MemoryStream(fileBytes);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
    private int GetIdOrDefault(object obj)
    {
        if (obj == null)
            return -1;

        Type type = obj.GetType();

        var prop =
            type.GetProperty("Id") ??
            type.GetProperty("ID") ??
            type.GetProperty("id");

        if (prop != null && prop.PropertyType == typeof(int))
        {
            return (int)prop.GetValue(obj);
        }

        var field =
            type.GetField("Id") ??
            type.GetField("ID") ??
            type.GetField("id");

        if (field != null && field.FieldType == typeof(int))
        {
            return (int)field.GetValue(obj);
        }

        return -1;
    }
}