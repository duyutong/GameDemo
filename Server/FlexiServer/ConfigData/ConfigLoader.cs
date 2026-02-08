using System.Collections.Concurrent;

namespace ConfigData
{
    public class ConfigLoader
    {
        private static string jsonPath = "ConfigData/Config/Json";
        private static ConcurrentDictionary<Type, IConfigDataHandler>? configDic;
        public static T? GetConfigData<T>(int id) where T : BaseConfig 
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
                string tablePath = Path.Combine(jsonPath, $"{className}.json");

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
}