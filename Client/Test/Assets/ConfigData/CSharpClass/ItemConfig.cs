
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"ItemConfig")]
public class ItemConfig : BaseConfig
{
	
    /// <summary>
    /// I
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public int ID;
    /// <summary>
    /// 图标
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public string Icon;
    /// <summary>
    /// 物品类型
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public int EItemType;
    /// <summary>
    /// 物品名称
    /// </summary>
    [global::ProtoBuf.ProtoMember(4)] public int ItemName;
    /// <summary>
    /// 物品描述
    /// </summary>
    [global::ProtoBuf.ProtoMember(5)] public int ItemDesc;
    
    public ItemConfig(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
    public ItemConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Icon = _dataDic["Icon"].ToString();
        EItemType = _dataDic["EItemType"].ToInt();
        ItemName = _dataDic["ItemName"].ToInt();
        ItemDesc = _dataDic["ItemDesc"].ToInt();
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 