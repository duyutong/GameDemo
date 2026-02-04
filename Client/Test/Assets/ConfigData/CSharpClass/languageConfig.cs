
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
public class LanguageConfig : BaseConfig
{
	
    /// <summary>
    /// 编号
    /// </summary>
    public int ID { get; protected set; }
    /// <summary>
    /// 中文
    /// </summary>
    public string Chinese { get; protected set; }
    /// <summary>
    /// 英文
    /// </summary>
    public string English { get; protected set; }

    public LanguageConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToInt();
        Chinese = _dataDic["Chinese"].ToString();
        English = _dataDic["English"].ToString();
        id = ID;
    }
} 