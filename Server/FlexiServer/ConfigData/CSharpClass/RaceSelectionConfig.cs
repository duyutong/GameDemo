
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
public class RaceSelectionConfig : BaseConfig
{
	
    /// <summary>
    /// 种族名称
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// 种族全身图
    /// </summary>
    public string FullPic { get; protected set; }
    /// <summary>
    /// 种族头像
    /// </summary>
    public string Icon { get; protected set; }
    /// <summary>
    /// （初始）生命值
    /// </summary>
    public int HP { get; protected set; }
    /// <summary>
    /// （初始）饱腹值
    /// </summary>
    public int Hunger { get; protected set; }
    /// <summary>
    /// （初始）含水值
    /// </summary>
    public int Hydration { get; protected set; }
    /// <summary>
    /// 角色简介
    /// </summary>
    public int Profile { get; protected set; }

    public RaceSelectionConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        FullPic = _dataDic["FullPic"].ToString();
        Icon = _dataDic["Icon"].ToString();
        HP = _dataDic["HP"].ToInt();
        Hunger = _dataDic["Hunger"].ToInt();
        Hydration = _dataDic["Hydration"].ToInt();
        Profile = _dataDic["Profile"].ToInt();
        id = ID;
    }
} 