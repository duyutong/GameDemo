using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace ConfigData
{
    public class ConfigLoader
    {
        private static string jsonPath = "Assets/AddressableAssets/Config/Json";
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
                string tablePath = Path.Combine(jsonPath, $"{className}.json");

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
}