
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
public class ItemConfig : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; protected set; }
    /// <summary>
    /// 物品类型
    /// </summary>
    public int EItemType { get; protected set; }
    /// <summary>
    /// 物品名称
    /// </summary>
    public int ItemName { get; protected set; }
    /// <summary>
    /// 物品描述
    /// </summary>
    public int ItemDesc { get; protected set; }

    public ItemConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Icon = _dataDic["Icon"].ToString();
        EItemType = _dataDic["EItemType"].ToInt();
        ItemName = _dataDic["ItemName"].ToInt();
        ItemDesc = _dataDic["ItemDesc"].ToInt();
        id = ID;
    }
} 