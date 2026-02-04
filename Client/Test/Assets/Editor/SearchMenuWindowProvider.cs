using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
/// <summary>
/// 提供搜索菜单窗口的功能
/// </summary>
public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
{
    public Func<List<SearchTreeEntry>> OnCreateSearchTreeAction;
    /// <summary>
    /// 选择搜索结果时触发的委托
    /// </summary>
    public delegate bool OnSelectEntryHandler(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

    /// <summary>
    /// 当选择搜索结果时的回调函数
    /// </summary>
    public OnSelectEntryHandler onSelectEntryHandler;

    /// <summary>
    /// 创建搜索树
    /// </summary>
    /// <param name="context">搜索窗口上下文</param>
    /// <returns>搜索树条目列表</returns>
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        return OnCreateSearchTreeAction.Invoke();
    }
    public List<SearchTreeEntry> GetProtocolEntries<T>(int level)
    {
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        var nodes = GetClassList(typeof(T));
        foreach (var _node in nodes)
        {
            if (!_node.IsSubclassOf(typeof(T))) continue;

            Type type = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .Select(a => a.GetType(_node.FullName))
                                    .FirstOrDefault(t => t != null);
            if (type == null) continue;

            string path = Path.Combine(NetworkPathConfig.GetClientApiFullPath(),$"{type.Name}.cs");
            entries.Add(new SearchTreeEntry(new GUIContent("  " + _node.Name)) { level = level, userData = path });
        }
        return entries;
    }
    public List<SearchTreeEntry> GetCommonValueTypeEntries(int level)
    {
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        List<string> types = CommonValueTypes.GetCommonTypeStrings();
        foreach (var type in types) entries.Add(new SearchTreeEntry(new GUIContent("  " + type)) { level = level, userData = type });
        return entries;
    }
    public List<SearchTreeEntry> GetModelEntries(int level) 
    {
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        //获取所有Model类型
        EditorUtilityExtensions.CheckRes(NetworkPathConfig.GetClientCommModelFullPath(), ".cs", (_path) =>
        {
            string exitModelName = Path.GetFileNameWithoutExtension(_path);
            entries.Add(new SearchTreeEntry(new GUIContent("  " + exitModelName)) { level = level, userData = null });
        });

        //还得加上被暂存的Model类型
        List<string> tempModels = NetworkCodeGenerator.self.GetTempModelNames();
        foreach (string temp in tempModels)
        {
            if (string.IsNullOrEmpty(temp)) continue;
            entries.Add(new SearchTreeEntry(new GUIContent("  " + temp)) { level = level, userData = null });
        }
        return entries;
    }
    /// <summary>
    /// 当选择搜索结果时调用
    /// </summary>
    /// <param name="searchTreeEntry">所选搜索树条目</param>
    /// <param name="context">搜索窗口上下文</param>
    /// <returns>是否成功处理了选择</returns>
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        if (onSelectEntryHandler == null) return false;
        return onSelectEntryHandler.Invoke(searchTreeEntry, context);
    }

    /// <summary>
    /// 获取指定类型的类列表
    /// </summary>
    /// <param name="type">基础类型</param>
    /// <returns>类列表</returns>
    private List<Type> GetClassList(Type type)
    {
        var q = type.Assembly.GetTypes()
             .Where(x => !x.IsAbstract)
             .Where(x => !x.IsGenericTypeDefinition)
             .Where(x => type.IsAssignableFrom(x));
        return q.ToList();
    }

}
public class CommonValueTypes
{
    private static readonly Dictionary<Type, string> CommonTypes = new()
    {
        { typeof(int), "int" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(long),"long"},
        { typeof(string), "string" },
        { typeof(bool), "bool" },
        { typeof(System.Numerics.Vector2), "Vector2" },
        { typeof(System.Numerics.Vector3), "Vector3" },
        { typeof(System.Numerics.Vector4), "Vector4" },
    };
    public static string GetShortNameByType(Type type)
    {
        if (CommonTypes.ContainsKey(type)) return CommonTypes[type];
        else return type.Name;
    }
    public static List<string> GetCommonTypeStrings()
    {
        var list = new List<string>();
        foreach (var kv in CommonTypes) list.Add(kv.Value);
        return list;
    }
}