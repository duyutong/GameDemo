
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
    public int HealthDecayRate { get; protected set; }
    /// <summary>
    /// 饱腹值下降速度（每5秒）
    /// </summary>
    public int HungerDecayRate { get; protected set; }
    /// <summary>
    /// 含水值下降速度（每5秒）
    /// </summary>
    public int HydrationDecayRate { get; protected set; }
    /// <summary>
    /// 以基础移速为基准的变化倍率
    /// </summary>
    public float MovementSpeed { get; protected set; }
    /// <summary>
    /// 物资获取倍率
    /// </summary>
    public float ResourceMultiplier { get; protected set; }
    /// <summary>
    /// 物资获取倍率触发概率（触发时获取物资倍率为“物资获取倍率，不触发时获取物资倍率为1）
    /// </summary>
    public float ResourceMultiplierProcChance { get; protected set; }
    
    public RaceInGameConfig() { }
    public override void Initialize(Dictionary<string, object> _dataDic)
    {
        ID = _dataDic["ID"].ToString();
        HP = _dataDic["HP"].ToInt();
        Hunger = _dataDic["Hunger"].ToInt();
        Hydration = _dataDic["Hydration"].ToInt();
        HealthDecayRate = _dataDic["Health Decay Rate"].ToInt();
        HungerDecayRate = _dataDic["Hunger Decay Rate"].ToInt();
        HydrationDecayRate = _dataDic["Hydration Decay Rate"].ToInt();
        MovementSpeed = _dataDic["Movement Speed"].ToFloat();
        ResourceMultiplier = _dataDic["Resource Multiplier"].ToFloat();
        ResourceMultiplierProcChance = _dataDic["Resource Multiplier Proc Chance"].ToFloat();
        
        id = ConfigLoaderUtil.ConvertToId(ID);
    }
} 