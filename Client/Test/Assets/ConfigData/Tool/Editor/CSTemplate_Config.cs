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
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @""#ClassName#"")]
public class #ClassName# : BaseConfig
{
	#ProContext#

    public #ClassName#() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {#InitContext#
        id = int.TryParse(ID, out var tempId) ? tempId : 0;
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
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
public class #ClassName# : BaseConfig
{
	#ProContext#

    public #ClassName#() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {#InitContext#
        id = ID;
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
        #ProName# = _dataDic[""#ProName#""].#MethodName#();";

    /// <summary>
    /// 配置表加载类
    /// </summary>
    public const string loaderClassStr_PB =
@"using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace ConfigData
{
    public class ConfigLoader
    {
        private string binaryPath =""Assets/AddressableAssets/Config/Binary"";
        private static Dictionary<Type, IConfigDataHandler> configDic;
        public static T GetConfigData<T>(int id) where T : BaseConfig 
        {
            if (configDic == null) InitConfigHandler();

            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;

            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData(id);

            return null;
        }
        private static void InitConfigHandler()
        {
            configDic = new Dictionary<Type, IConfigDataHandler>();

            List<Type> types = GetClassList<BaseConfig>();

            foreach (var configType in types)
            {
                if (!configType.IsSubclassOf(typeof(BaseConfig)))
                    continue;

                string className = configType.Name;
                string tablePath = Path.Combine(binaryPath, $""{className}.bytes"");

                Type handlerType = typeof(ConfigDataBinary<>).MakeGenericType(configType);
                var handler = (IConfigDataHandler)Activator.CreateInstance(handlerType);
                handler.SetTablePath(tablePath);

                configDic.Add(configType, handler);
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
@"using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace ConfigData
{
    public class ConfigLoader
    {
        private static string jsonPath = ""Assets/AddressableAssets/Config/Json"";
        private static Dictionary<Type, IConfigDataHandler> configDic;
        public static T GetConfigData<T>(int id) where T : BaseConfig 
        {
            if (configDic == null) InitConfigHandler();

            if (!configDic.TryGetValue(typeof(T), out var handler)) return null;

            if (handler is IConfigDataHandler<T> typedHandler)
                return typedHandler.GetConfigData(id);
            
            return null;
        }

        private static void InitConfigHandler()
        {
            configDic = new Dictionary<Type, IConfigDataHandler>();

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

                configDic.Add(configType, handler);
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
