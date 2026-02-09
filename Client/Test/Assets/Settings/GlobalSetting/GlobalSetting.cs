using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSetting", menuName = "Scriptable Objects/GlobalSetting")]
public class GlobalSetting : ScriptableObject
{
    private static GlobalSetting _instance;
    private static string assetPath = "Assets/Settings/GlobalSetting/GlobalSetting.asset";
    public ELanguage language = ELanguage.Chinese;
    public static GlobalSetting Instance
    {
        get
        {
            if (_instance == null)
                ResourceLoader.Load<GlobalSetting>(assetPath, (_setting) => _instance = _setting);
            return _instance;
        }
    }
}
public enum ELanguage
{
    Chinese = 0,
    English = 1,
}