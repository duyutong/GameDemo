
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"RaceSelectionConfig")]
public class RaceSelectionConfig : BaseConfig
{
	
    /// <summary>
    /// 种族名称
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public int ID;
    /// <summary>
    /// 种族全身图
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public string FullPic;
    /// <summary>
    /// 种族头像
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public string Icon;
    /// <summary>
    /// （初始）生命值
    /// </summary>
    [global::ProtoBuf.ProtoMember(4)] public int HP;
    /// <summary>
    /// （初始）饱腹值
    /// </summary>
    [global::ProtoBuf.ProtoMember(5)] public int Hunger;
    /// <summary>
    /// （初始）含水值
    /// </summary>
    [global::ProtoBuf.ProtoMember(6)] public int Hydration;
    /// <summary>
    /// 角色简介
    /// </summary>
    [global::ProtoBuf.ProtoMember(7)] public int Profile;
    
    public RaceSelectionConfig(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
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
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 