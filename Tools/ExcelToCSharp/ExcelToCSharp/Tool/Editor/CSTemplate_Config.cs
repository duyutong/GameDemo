/// <summary>
/// 自动生成cs文件时使用的预制文字
/// </summary>
public class CSTemplate_Config
{
    /// <summary>
    /// json生成的配置表类
    /// </summary>
    public const string classStr_PB =
@"
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @""#ClassName#"")]
public class #ClassName# : BaseConfig
{
	#ProContext#
    
    public #ClassName#(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
    public #ClassName#() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {#InitContext#
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} ";
    /// <summary>
    /// 配置表类的属性部分
    /// </summary>
    public const string proStr_PB =
    @"
    /// <summary>
    /// #ProDes#
    /// </summary>
    [global::ProtoBuf.ProtoMember(#Pos#)] public #ProType# #ProName#;";
    /// <summary>
    /// json生成的配置表类
    /// </summary>
    public const string classStr_Json =
@"
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
public class #ClassName# : BaseConfig
{
	#ProContext#
    
    public #ClassName#() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {#InitContext#
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} ";
    /// <summary>
    /// 配置表类的属性部分
    /// </summary>
    public const string proStr_Json =
    @"
    /// <summary>
    /// #ProDes#
    /// </summary>
    public #ProType# #ProName# { get; protected set; }";

    /// <summary>
    /// 配置表类的Init函数内容
    /// </summary>
    public const string classInitStr =
        @"
        #ProName# = _dataDic[""#ProKey#""]#MethodName#;";

    /// <summary>
    /// 配置表加载类
    /// </summary>
    public const string loaderClassStr_PB =
@"using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConfigData
{
    public class ConfigLoader
    {
        private static string binaryPath =""ConfigData/Config/Binary"";
        private static ConcurrentDictionary<Type, IConfigDataHandler>? configDic;
        public static T GetConfigData<T>(string id) where T : BaseConfig 
        {
            int idKey = ConfigLoaderUtil.ConvertToId(id);
            return GetConfigData<T>(idKey);
        }
        public static T GetConfigData<T>(int id) where T : BaseConfig 
        {
            if (configDic == null) InitConfigHandler();

            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;

            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData(id);

            return null;
        }
        public static List<T> GetConfigDatas<T>(int count) where T : BaseConfig
        {
            if (configDic == null) InitConfigHandler();

            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;

            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData((_conf) => true, count);
            
            return null;
        }
        private static void InitConfigHandler()
        {
            configDic = new();

            List<Type> types = GetClassList<BaseConfig>();

            foreach (var configType in types)
            {
                if (!configType.IsSubclassOf(typeof(BaseConfig)))
                    continue;

                string className = configType.Name;
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string tablePath = Path.Combine(binaryPath, $""{className}.bytes"");

                Type handlerType = typeof(ConfigDataBinary<>).MakeGenericType(configType);
                var handler = (IConfigDataHandler)Activator.CreateInstance(handlerType);
                handler.SetTablePath(tablePath);

                configDic.TryAdd(configType, handler);
            }
        }

        private static List<Type> GetClassList<T>()
        {
            Type type = typeof (T);
            var q = type.Assembly.GetTypes()
                 .Where(x => !x.IsAbstract)
                 .Where(x => !x.IsGenericTypeDefinition)
                 .Where(x => type.IsAssignableFrom(x));
            return q.ToList();
        }
    }
}
";
    /// <summary>
    /// 配置表加载类
    /// </summary>
    public const string loaderClassStr_Json =
@"using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConfigData
{
    public class ConfigLoader
    {
        private static string jsonPath = ""ConfigData/Config/Json"";
        private static ConcurrentDictionary<Type, IConfigDataHandler>? configDic;
        public static T GetConfigData<T>(string id) where T : BaseConfig 
        {
            int idKey = ConfigLoaderUtil.ConvertToId(id);
            return GetConfigData<T>(idKey);
        }
        public static T GetConfigData<T>(int id) where T : BaseConfig 
        {
            if (configDic == null) InitConfigHandler();

            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;

            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData(id);
            
            return null;
        }
        public static List<T> GetConfigDatas<T>(int count) where T : BaseConfig
        {
            if (configDic == null) InitConfigHandler();
            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;
            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData((_conf) => true, count);
            
            return null;
        }

        private static void InitConfigHandler()
        {
            configDic = new();

            List<Type> types = GetClassList<BaseConfig>();

            foreach (var configType in types)
            {
                if (!configType.IsSubclassOf(typeof(BaseConfig)))
                    continue;

                string className = configType.Name;
                string tablePath = Path.Combine(jsonPath, $""{className}.json"");

                Type handlerType = typeof(ConfigDataJson<>).MakeGenericType(configType);
                
                var handler = (IConfigDataHandler)Activator.CreateInstance(handlerType);
                handler.SetTablePath(tablePath);

                configDic.TryAdd(configType, handler);
            }
        }
        
        private static List<Type> GetClassList<T>()
        {
            Type type = typeof (T);
            var q = type.Assembly.GetTypes()
                 .Where(x => !x.IsAbstract)
                 .Where(x => !x.IsGenericTypeDefinition)
                 .Where(x => type.IsAssignableFrom(x));
            return q.ToList();
        }
    }
}";
}
