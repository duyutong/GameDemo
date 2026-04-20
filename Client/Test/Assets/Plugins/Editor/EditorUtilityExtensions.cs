using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class EditorUtilityExtensions
{
    public static string GetMD5(string folderPath)
    {
        using (FileStream Folder = File.OpenRead(folderPath))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes = md5.ComputeHash(Folder);
            Folder.Close();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes) stringBuilder.Append(b.ToString("x2"));

            return stringBuilder.ToString();
        }
    }
    public static Dictionary<Type, List<string>> GetPublicMembers(this Type type)
    {
        var dict = new Dictionary<Type, List<string>>();

        // 1. 公有字段
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            AddToDict(dict, field.FieldType, field.Name);
        }

        // 2. 公有属性
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        foreach (var prop in props)
        {
            // 排除没有getter的属性
            if (!prop.CanRead) continue;
            AddToDict(dict, prop.PropertyType, prop.Name);
        }

        return dict;
    }

    private static void AddToDict(Dictionary<Type, List<string>> dict, Type type, string name)
    {
        if (!dict.TryGetValue(type, out var list))
        {
            list = new List<string>();
            dict[type] = list;
        }
        list.Add(name);
    }
    public static void CheckRes(string path, string extension, Action<string> action = null)
    {
        if (string.IsNullOrEmpty(path)) return;
        if (File.Exists(path))
        {
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Extension == extension) action?.Invoke(fileInfo.FullName);
        }
        else
        {
            string[] files = Directory.GetFiles(path, $"*{extension}", SearchOption.AllDirectories);
            foreach (string file in files) action?.Invoke(file);
        }
    }
    public static string ToShortPath(this string fullPath)
    {
#if UNITY_IOS
        string pattern = @"Assets/(.+)";
#else
        string pattern = @"Assets\\(.+)";
#endif
        Match match = Regex.Match(fullPath, pattern, RegexOptions.IgnoreCase);

        if (!match.Success) return fullPath;
        return match.Groups[0].Value;

    }
    public static string GetPlatformName()
    {
        RuntimePlatform platform = Application.platform;

        string platformName;

        switch (platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                platformName = "Windows";
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                platformName = "Mac OS X";
                break;
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.LinuxPlayer:
                platformName = "Linux";
                break;
            case RuntimePlatform.Android:
                platformName = "Android";
                break;
            case RuntimePlatform.IPhonePlayer:
                platformName = "iOS";
                break;
            case RuntimePlatform.WebGLPlayer:
                platformName = "WebGL";
                break;
            default:
                platformName = "Unknown";
                break;
        }

        return platformName;
    }
    public static void RemoveDuplicateItems<T1, T2>(this IList<T1> ts, Func<T1, T2> checkFunc)
    {
        IList<T2> temp = new List<T2>();
        List<int> indexList = new List<int>();

        for (int i = 0; i < ts.Count; i++)
        {
            T1 t1 = ts[i];
            T2 t2 = checkFunc(t1);
            if (temp.Contains(t2)) indexList.Add(i);
            else temp.Add(t2);
        }

        indexList.Sort((a, b) => b.CompareTo(a));

        for (int i = 0; i < indexList.Count; i++)
        {
            if (ts.Count <= indexList[i]) continue;
            ts.RemoveAt(indexList[i]);
        }
    }
    public static void SaveCSFile(string csSavePath, string content)
    {
        if (string.IsNullOrEmpty(csSavePath) || string.IsNullOrEmpty(content)) return;

        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        string directory = Path.GetDirectoryName(csSavePath);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        File.WriteAllBytes(csSavePath, contentBytes);
        AssetDatabase.Refresh();
    }
    public static void CopyFile(string sourceFilePath, string destinationPath, string newFileName = "")
    {
        string fileName = string.IsNullOrEmpty(newFileName) ? Path.GetFileName(sourceFilePath) : newFileName;
        string destinationFilePath = Path.Combine(destinationPath, fileName);
        if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
        File.Copy(sourceFilePath, destinationFilePath, true);
    }
    // 移动文件到指定文件夹
    public static void MoveFileToFolder(string filePath, string folderPath)
    {
        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"文件不存在: {filePath}");
            return;
        }

        // 检查文件夹是否存在，如果不存在则创建它
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
            Debug.Log($"创建文件夹: {folderPath}");
        }

        // 移动文件到目标文件夹
        string destinationPath = Path.Combine(folderPath, Path.GetFileName(filePath));

        Debug.Log($"移动文件: {destinationPath} 结果：{AssetDatabase.MoveAsset(filePath.ToShortPath(), destinationPath)}");
    }
    // 删除指定路径的文件
    public static void DeleteFile(string filePath)
    {
        // 检查文件是否存在
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"文件已删除: {filePath}");
        }
        else
        {
            Debug.LogWarning($"文件不存在: {filePath}");
        }
    }
    // 删除指定路径下的空文件夹
    public static void DeleteEmptyFolders(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
        {
            Debug.LogWarning($"指定的目录不存在: {rootDirectory}");
            return;
        }

        // 获取所有子文件夹
        string[] subdirectories = Directory.GetDirectories(rootDirectory);

        foreach (string directory in subdirectories)
        {
            // 递归删除空文件夹
            DeleteEmptyFolders(directory);
        }

        // 检查当前文件夹是否为空
        if (Directory.GetFiles(rootDirectory).Length == 0 && Directory.GetDirectories(rootDirectory).Length == 0)
        {
            Directory.Delete(rootDirectory);
            File.Delete(rootDirectory + ".meta");
            Debug.Log($"已删除空文件夹: {rootDirectory}");
        }
    }
    public static string GetFileName(string path, bool withExtension = false)
    {
        // 使用Path类的GetFileName方法来获取文件名
        string fileName = Path.GetFileName(path);

        // 如果不需要包含文件扩展名，则去除扩展名部分
        if (!withExtension)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
        }

        return fileName;
    }
    /// <summary>
    /// 从 source 中提取被 start 和 end 包裹的内容（包含标记之间的内容，不包含标记本身）。
    /// </summary>
    /// <param name="start">起始标记字符串（例如 &lt;tag&gt;）</param>
    /// <param name="end">结束标记字符串（例如 &lt;/tag&gt;）</param>
    /// <param name="source">待搜索的原始字符串</param>
    /// <returns>
    /// 返回 start 和 end 之间的内容：
    /// - 若匹配成功：返回去除首尾空行后的中间内容（不包含 start 和 end 本身）
    /// - 若未匹配或参数为空：返回空字符串
    /// </returns>
    /// <remarks>
    /// 特性：
    /// 1. 自动对 start 和 end 做 Regex 转义，避免特殊字符影响匹配
    /// 2. 使用非贪婪匹配 (.*?)，只匹配最近的一对 start/end
    /// 3. 支持跨行匹配（Singleline 模式）
    /// 4. 会自动去除提取内容前后的空行和纯空白行
    /// </remarks>
    public static string ExtractInnerContent(string start, string end, string source)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end) || string.IsNullOrEmpty(source))
            return "";

        string escStart = Regex.Escape(start);
        string escEnd = Regex.Escape(end);

        // 匹配 start 和 end 之间的所有内容，允许换行
        string pattern = $@"{escStart}(.*?){escEnd}";

        var match = Regex.Match(source, pattern, RegexOptions.Singleline);
        if (!match.Success) return "";

        // 取出内容
        string content = match.Groups[1].Value;

        // 去掉开头和结尾的空行 / 空白
        var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        var trimmedLines = lines.SkipWhile(l => string.IsNullOrWhiteSpace(l))
                                .Reverse()
                                .SkipWhile(l => string.IsNullOrWhiteSpace(l))
                                .Reverse();

        return string.Join("\n", trimmedLines);
    }
    /// <summary>
    /// 从 source 中提取被 start 和 end 包裹的内容（包含标记），
    /// 并替换 target 中对应的内容。
    /// </summary>
    /// <param name="start">起始标记</param>
    /// <param name="end">结束标记</param>
    /// <param name="source">提取来源</param>
    /// <param name="target">被替换目标</param>
    /// <returns>替换后的字符串；未匹配则返回原 target</returns>
    public static string ReplaceWrappedContent(string start, string end, string source, string target)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end) ||
            string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return target;

        // 转义，防止特殊字符影响正则
        string escStart = Regex.Escape(start);
        string escEnd = Regex.Escape(end);

        // 匹配被包裹的内容（包含start和end）
        string pattern = $"{escStart}.*?{escEnd}";

        // 从 source 提取
        var match = Regex.Match(source, pattern);
        if (!match.Success) return target;

        string extracted = match.Value;

        // 替换 target 中的对应部分
        return Regex.Replace(target, pattern, extracted);
    }
    /// <summary>
    /// 检查指定文件是否存在，如果存在则读取文件内容
    /// </summary>
    /// <param name="filePath">文件路径，可以是绝对路径或相对路径</param>
    /// <returns>文件内容，如果文件不存在或读取失败则返回 null</returns>
    public static string ReadFileIfExists(string filePath)
    {
        // 判断文件是否存在
        if (!File.Exists(filePath)) return string.Empty;
        try
        {
            // 强制按 UTF8 无 BOM 读取
            string content = File.ReadAllText(filePath);
            return content;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取文件失败: {ex.Message}");
            return string.Empty;
        }
    }
    /// <summary>
    /// 将输入的字符串转换为小驼峰（camelCase）和大驼峰（PascalCase）格式。
    /// 支持下划线、中划线、空格、以及大小写边界（如 createRoom、XMLParser 等）。
    /// </summary>
    /// <param name="input">要转换的输入字符串，可以包含下划线、连字符、空格或混合大小写。</param>
    /// <param name="camelCase">输出的小驼峰格式字符串（首字母小写或首个缩写小写，其余单词首字母大写）。</param>
    /// <param name="pascalCase">输出的大驼峰格式字符串（首字母大写，其余单词首字母大写，缩写保留大写）。</param>
    public static void ToCamelAndPascal(string input, out string camelCase, out string pascalCase)
    {
        camelCase = pascalCase = string.Empty;
        if (string.IsNullOrWhiteSpace(input))
            return;

        // 先把常见分隔符替成空格，方便按大小写规则切分
        var cleaned = Regex.Replace(input, @"[\s\-_\/\.]+", " ").Trim();

        // 这个正则用于按 camel/Pascal/ACRONYM/数字 切分单词
        // 匹配规则说明：
        //  - [A-Z]+(?=[A-Z][a-z])   匹配像 "XML" 在 "XMLHttp" 这种前瞻场景中的前半段
        //  - [A-Z]?[a-z]+           匹配普通单词（可能首字母大写）
        //  - [A-Z]+(?![a-z])        匹配全大写缩写（如 "ID"、"XML"）
        //  - \d+                    匹配数字
        var matches = Regex.Matches(cleaned, @"[A-Z]+(?=[A-Z][a-z])|[A-Z]?[a-z]+|[A-Z]+(?![a-z])|\d+")
                           .Cast<Match>()
                           .Select(m => m.Value)
                           .ToList();

        if (matches.Count == 0)
            return;

        // 生成 PascalCase：缩写保持大写，普通单词首字母大写其余小写
        pascalCase = string.Concat(matches.Select(w =>
        {
            if (w.All(char.IsUpper)) // ACRONYM
                return w;
            // 普通单词：首字母大写，其余小写
            return char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w.Substring(1).ToLowerInvariant() : string.Empty);
        }));

        // 生成 camelCase：
        // 如果开头是多个大写字母的缩写（如 XMLParser），将整个缩写小写（xmlParser）。
        // 否则，将首字母小写（CreateRoom -> createRoom）。
        if (string.IsNullOrEmpty(pascalCase))
        {
            camelCase = string.Empty;
            return;
        }

        var acrMatch = Regex.Match(pascalCase, @"^[A-Z]{2,}");
        if (acrMatch.Success)
        {
            var acr = acrMatch.Value;
            camelCase = acr.ToLowerInvariant() + pascalCase.Substring(acr.Length);
        }
        else
        {
            camelCase = char.ToLowerInvariant(pascalCase[0]) + (pascalCase.Length > 1 ? pascalCase.Substring(1) : string.Empty);
        }
    }
    /// <summary>
    /// 根据类型名获取类型
    /// </summary>
    public static Type GetType(string typeName)
    {
        string assemblyName = "Assembly-CSharp"; // 程序集名称
        Type type = null;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == assemblyName)
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
        }
        return type;
    }
    public static bool TryGetListElementType(PropertyInfo prop, out Type elementType)
    {
        Type type = prop.PropertyType;

        if (type.IsGenericType &&
            typeof(System.Collections.IEnumerable).IsAssignableFrom(type) &&
            type.GetGenericTypeDefinition() == typeof(List<>))
        {
            elementType = type.GetGenericArguments()[0]; // 列表元素类型
            return true;
        }

        elementType = null;
        return false;
    }
    /// <summary>
    /// 根据传入的 PersistentData 手动构建一个带有持久化监听的 UnityEvent。
    /// </summary>
    /// <param name="persistentData">
    /// 持久化事件数据，包含目标组件类型（assemblyTypeName）、目标对象（target）以及方法名（methodName）
    /// </param>
    /// <param name="index">插入到持久化调用列表中的索引位置（默认插入到第 0 位）</param>
    /// <returns>
    /// 返回构建完成的 UnityEvent：
    /// - 成功：返回包含一个 PersistentCall 的 UnityEvent
    /// - 失败（如找不到目标组件）：返回 null
    /// </returns>
    /// <remarks>
    /// 实现说明：
    /// 1. 该方法通过反射直接访问 UnityEvent 内部结构（UnityEventBase -> PersistentCallGroup -> PersistentCall）
    /// 2. 手动创建 PersistentCall，并写入 m_Target、m_MethodName、m_TargetAssemblyTypeName 等私有字段
    /// 3. 最终插入到 m_Calls 列表中，从而构建出一个“持久化事件”
    ///
    /// 注意事项：
    /// - 该实现依赖 Unity 内部私有字段（如 m_PersistentCalls、m_Calls），属于非公开 API，存在版本兼容风险
    /// - 代码较为底层且绕弯，不易维护，也不具备良好的可读性
    /// - 若 Unity 内部实现发生变化，此方法可能失效
    ///
    /// 强烈建议：
    /// - 优先使用 Unity 官方提供的编辑器工具方法：
    ///   UnityEditor.Events.UnityEventTools.AddPersistentListener(...)
    /// - 该 API 更安全、直观，并且兼容性更好
    /// </remarks>
    public static UnityEvent IntegrateEventInfo(PersistentData persistentData, int index = 0)
    {
        UnityEvent targetEvent = new UnityEvent();

        // 获取 UnityEventBase 内部的 m_PersistentCalls
        var baseType = typeof(UnityEventBase); // UnityEventBase 是 UnityEvent 的基类
        var callsField = baseType.GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
        var persistentCalls = callsField.GetValue(targetEvent);

        // 获取 PersistentCallGroup 中的 m_Calls 列表
        var callListType = persistentCalls.GetType();
        var callsFieldInGroup = callListType.GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);

        // 使用反射获取 List<PersistentCall>
        var calls = callsFieldInGroup.GetValue(persistentCalls) as IList;

        // 获取指定索引的 PersistentCall
        Type persistentCallType = Type.GetType("UnityEngine.Events.PersistentCall,UnityEngine.CoreModule");
        var persistentCall = Activator.CreateInstance(persistentCallType);

        // 通过反射获取 PersistentCall 的相关字段
        FieldInfo targetAssemblyTypeNameField = persistentCallType.GetField("m_TargetAssemblyTypeName", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo target = persistentCallType.GetField("m_Target", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo methodName = persistentCallType.GetField("m_MethodName", BindingFlags.NonPublic | BindingFlags.Instance);

        //赋值
        targetAssemblyTypeNameField.SetValue(persistentCall, persistentData.assemblyTypeName);
        UnityEngine.Object targetComponent = (persistentData.target as GameObject).GetComponent(persistentData.assemblyTypeName);

        if (targetComponent == null) return null;
        target.SetValue(persistentCall, targetComponent);
        methodName.SetValue(persistentCall, persistentData.methodName);

        calls.Insert(index, persistentCall);

        callsFieldInGroup.SetValue(persistentCalls, calls);

        return targetEvent;
    }
    public class PersistentData
    {
        public UnityEngine.Object target;   // 目标 GameObject
        public string assemblyTypeName;     // 类名
        public string methodName;           // 方法名
    }
}

