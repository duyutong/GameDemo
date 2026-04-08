using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 遵守配置表规则前提下，用于类型转换的工具类
/// </summary>
public static class ParseUtil
{
    /// <summary>
    /// 将配置内容转换为枚举值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this object self)
    {
        if (int.TryParse(self.ToString(), out int intValue)) return default(T);
        return (T)Enum.ToObject(typeof(T), intValue);
    }
    /// <summary>
    /// 将配置内容转换为整型
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static int ToInt(this object self)
    {
        string temp = self.ToString().Split('.')[0];
        return int.Parse(temp);
    }
    /// <summary>
    /// 将配置内容转换为浮点数
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static float ToFloat(this object self)
    {
        return float.Parse(self.ToString());
    }
    /// <summary>
    /// 将配置内容转换为整型列表
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static List<T> ToArray<T>(this object self)
    {
        List<T> array = new List<T>();
        string[] temp = self.ToString().Split(',');
        for (int i = 0; i < temp.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(temp[i])) continue;

            T value = (T)Convert.ChangeType(temp[i], typeof(T));
            array.Add(value);
        }
        return array;
    }
    /// <summary>
    /// 将配置内容转换为值为整型的二维列表
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static List<List<T>> ToArrays<T>(this object self)
    {
        List<List<T>> arrays = new List<List<T>>();
        string[] temp1 = self.ToString().Split(';');
        for (int i = 0; i < temp1.Length; i++)
        {
            string[] temp2 = temp1[i].Split(',');
            arrays.Add(new List<T>());
            for (int j = 0; j < temp2.Length; j++)
            {
                T value = (T)Convert.ChangeType(temp2[j], typeof(T));
                arrays[i].Add(value);
            }
        }
        return arrays;
    }
    /// <summary>
    /// 讲配置内容转换为key和value都是整型的字典
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this object self)
    {
        Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
        string[] temp1 = self.ToString().Split(',');
        for (int i = 0; i < temp1.Length; i++)
        {
            string[] temp2 = temp1[i].Split('=');
            TKey key = (TKey)Convert.ChangeType(temp2[0], typeof(TKey));
            TValue value = (TValue)Convert.ChangeType(temp2[1], typeof(TValue));
            dic[key] = value;
        }
        return dic;
    }
    /// <summary>
    /// 转换数据类型时，按照配置的类型选择转换所需要调用的函数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetMethodName(this string type)
    {
        if (TryParseList(type, out string elementType)) return $".ToArray<{elementType}>()";
        if (TryParseNestedList(type, out string elementType1)) return $".ToArrays<{elementType1}>()";
        if (TryParseDictionary(type, out string keyType, out string valueType)) return $".ToDictionary<{keyType}, {valueType}>()";

        if (type == "string") return ".ToString()";
        if (type == "int") return ".ToInt()";
        if (type == "float") return ".ToFloat()";
        if (CheckIsEnum(type)) return $".ToEnum<{type}>()";

        return $" as {type}";
    }
    public static bool TryParseDictionary(string input, out string keyType, out string valueType)
    {
        keyType = null;
        valueType = null;

        var match = Regex.Match(input, @"^Dictionary<\s*(\w+)\s*,\s*(\w+)\s*>$");
        if (!match.Success) return false;

        keyType = match.Groups[1].Value;
        valueType = match.Groups[2].Value;
        return true;
    }
    public static bool TryParseList(string input, out string elementType)
    {
        elementType = null;

        var match = Regex.Match(input, @"^List<\s*(\w+)\s*>$");
        if (!match.Success) return false;

        elementType = match.Groups[1].Value;
        return true;
    }
    public static bool TryParseNestedList(string input, out string elementType)
    {
        elementType = null;

        var match = Regex.Match(input, @"^List<\s*List<\s*(\w+)\s*>\s*>$");
        if (!match.Success) return false;

        elementType = match.Groups[1].Value;
        return true;
    }
    public static bool CheckIsEnum(string enumName)
    {
        // 规则：E + 首字母大写的若干字符 + Type
        bool isType = Regex.IsMatch(enumName, @"^E[A-Z].*Type$");
        bool isState = Regex.IsMatch(enumName, @"^E[A-Z].*State$");
        return isType || isState;
    }
}
