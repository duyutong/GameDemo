
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
public class RaceInGameConfig : BaseConfig
{
	
    /// <summary>
    /// 种族名称
    /// </summary>
    public string ID { get; protected set; }
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
    /// 生命值下降速度（每5秒）
    /// </summary>
    public int Health Decay Rate { get; protected set; }
    /// <summary>
    /// 饱腹值下降速度（每5秒）
    /// </summary>
    public int Hunger Decay Rate { get; protected set; }
    /// <summary>
    /// 含水值下降速度（每5秒）
    /// </summary>
    public int Hydration Decay Rate { get; protected set; }
    /// <summary>
    /// 以基础移速为基准的变化倍率
    /// </summary>
    public float Movement Speed { get; protected set; }
    /// <summary>
    /// 物资获取倍率
    /// </summary>
    public float Resource Multiplier { get; protected set; }
    /// <summary>
    /// 物资获取倍率触发概率（触发时获取物资倍率为“物资获取倍率，不触发时获取物资倍率为1）
    /// </summary>
    public float Resource Multiplier Proc Chance { get; protected set; }

    public RaceInGameConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToString();
        HP = _dataDic["HP"].ToInt();
        Hunger = _dataDic["Hunger"].ToInt();
        Hydration = _dataDic["Hydration"].ToInt();
        Health Decay Rate = _dataDic["Health Decay Rate"].ToInt();
        Hunger Decay Rate = _dataDic["Hunger Decay Rate"].ToInt();
        Hydration Decay Rate = _dataDic["Hydration Decay Rate"].ToInt();
        Movement Speed = _dataDic["Movement Speed"].ToString();
        Resource Multiplier = _dataDic["Resource Multiplier"].ToString();
        Resource Multiplier Proc Chance = _dataDic["Resource Multiplier Proc Chance"].ToString();
        id = ID;
    }
} 