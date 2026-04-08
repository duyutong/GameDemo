
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
public class MyNewExcel : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// 一维数组
    /// </summary>
    public List<int> Array { get; protected set; }
    /// <summary>
    /// 二维数组
    /// </summary>
    public List<List<int>> Arrays { get; protected set; }
    /// <summary>
    /// 键值对
    /// </summary>
    public Dictionary<int,int> Pair { get; protected set; }
    /// <summary>
    /// 字符串
    /// </summary>
    public string String { get; protected set; }
    
    public MyNewExcel() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Array = _dataDic["Array"].ToArray<int>();
        Arrays = _dataDic["Arrays"].ToArrays<int>();
        Pair = _dataDic["Pair"].ToDictionary<int, int>();
        String = _dataDic["String"].ToString();
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 