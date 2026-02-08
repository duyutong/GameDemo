using UnityEngine;
using ConfigData;
public class ConfigLoaderUtil
{
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
}
