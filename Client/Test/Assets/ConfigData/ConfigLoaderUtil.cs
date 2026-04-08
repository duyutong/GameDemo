using UnityEngine;
using ConfigData;
public class ConfigLoaderUtil
{
    public static string GetLanguageById(string id) 
    {
        int idKey= id.GetHashCode();
        return GetLanguageById(idKey);
    }
    public static string GetLanguageById(int id)
    {
        LanguageConfig config = ConfigLoader.GetConfigData<LanguageConfig>(id);
        if (config == null) return null;

        return GlobalSetting.Instance.language switch
        {
            ELanguage.Chinese => config?.Chinese,
            ELanguage.English => config?.English,
            _ => config?.Chinese,
        };
    }
    public static int ConvertToId(object value)
    {
        if (value == null) return -1;

        // 如果本身就是 int
        if (value is int i) return i;

        //常见数值类型（避免直接 GetHashCode）
        if (value is long l) return (int)l;
        if (value is float f) return (int)f;
        if (value is double d) return (int)d;

        // 字符串：优先尝试转 int
        if (value is string s)
        {
            if (int.TryParse(s, out var result)) return result;
            return s.GetHashCode();
        }

        // 其他类型：统一走 HashCode
        return value.GetHashCode();
    }
}
