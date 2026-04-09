using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConfigData
{
    public class ConfigLoader
    {
        private static string binaryPath ="Assets/AddressableAssets/Config/Binary";
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
                string tablePath = Path.Combine(binaryPath, $"{className}.bytes");

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
