
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"ClueConfig")]
public class ClueConfig : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public int ID;
    /// <summary>
    /// 线索的对应物品id
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public int ItemId;
    /// <summary>
    /// 线索类型
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public EClueType ClueType;
    /// <summary>
    /// 最高分值
    /// </summary>
    [global::ProtoBuf.ProtoMember(4)] public int ScoreMax;
    /// <summary>
    /// 最低分值
    /// </summary>
    [global::ProtoBuf.ProtoMember(5)] public int ScoreMin;
    /// <summary>
    /// 图鉴描述
    /// </summary>
    [global::ProtoBuf.ProtoMember(6)] public int ClueDes;
    /// <summary>
    /// 线索展示图
    /// </summary>
    [global::ProtoBuf.ProtoMember(7)] public string ClueTex;
    
    public ClueConfig(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
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
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 