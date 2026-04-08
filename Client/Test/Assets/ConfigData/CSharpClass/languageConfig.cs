
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"LanguageConfig")]
public class LanguageConfig : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public int ID;
    /// <summary>
    /// 中文
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public string Chinese;
    /// <summary>
    /// 英文
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public string English;
    
    public LanguageConfig(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
    public LanguageConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Chinese = _dataDic["Chinese"].ToString();
        English = _dataDic["English"].ToString();
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 