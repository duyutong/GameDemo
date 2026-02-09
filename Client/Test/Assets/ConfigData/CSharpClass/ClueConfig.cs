
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
public class ClueConfig : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// 线索的对应物品id
    /// </summary>
    public int ItemId { get; protected set; }
    /// <summary>
    /// 线索类型
    /// </summary>
    public EClueType ClueType { get; protected set; }
    /// <summary>
    /// 最高分值
    /// </summary>
    public int ScoreMax { get; protected set; }
    /// <summary>
    /// 最低分值
    /// </summary>
    public int ScoreMin { get; protected set; }
    /// <summary>
    /// 图鉴描述
    /// </summary>
    public int ClueDes { get; protected set; }
    /// <summary>
    /// 线索展示图
    /// </summary>
    public string ClueTex { get; protected set; }

    public ClueConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        ItemId = _dataDic["ItemId"].ToInt();
        ClueType = _dataDic["ClueType"].ToEnum<EClueType>();
        ScoreMax = _dataDic["ScoreMax"].ToInt();
        ScoreMin = _dataDic["ScoreMin"].ToInt();
        ClueDes = _dataDic["ClueDes"].ToInt();
        ClueTex = _dataDic["ClueTex"].ToString();
        id = ID;
    }
} 