
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"MyNewExcel")]
public class MyNewExcel : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public int ID;
    /// <summary>
    /// 一维数组
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public List<int> Array;
    /// <summary>
    /// 键值对
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public Dictionary<int,int> Pair;
    /// <summary>
    /// 字符串
    /// </summary>
    [global::ProtoBuf.ProtoMember(4)] public string String;
    
    public MyNewExcel(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
    public MyNewExcel() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Array = _dataDic["Array"].ToArray<int>();
        Pair = _dataDic["Pair"].ToDictionary<int, int>();
        String = _dataDic["String"].ToString();
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 