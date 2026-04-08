
using System.Collections.Generic;
using static EnumDefinitions;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @"RaceInGameConfig")]
public class RaceInGameConfig : BaseConfig
{
	
    /// <summary>
    /// 种族名称
    /// </summary>
    [global::ProtoBuf.ProtoMember(1)] public string ID;
    /// <summary>
    /// （初始）生命值
    /// </summary>
    [global::ProtoBuf.ProtoMember(2)] public int HP;
    /// <summary>
    /// （初始）饱腹值
    /// </summary>
    [global::ProtoBuf.ProtoMember(3)] public int Hunger;
    /// <summary>
    /// （初始）含水值
    /// </summary>
    [global::ProtoBuf.ProtoMember(4)] public int Hydration;
    /// <summary>
    /// 生命值下降速度（每5秒）
    /// </summary>
    [global::ProtoBuf.ProtoMember(5)] public int HealthDecayRate;
    /// <summary>
    /// 饱腹值下降速度（每5秒）
    /// </summary>
    [global::ProtoBuf.ProtoMember(6)] public int HungerDecayRate;
    /// <summary>
    /// 含水值下降速度（每5秒）
    /// </summary>
    [global::ProtoBuf.ProtoMember(7)] public int HydrationDecayRate;
    /// <summary>
    /// 以基础移速为基准的变化倍率
    /// </summary>
    [global::ProtoBuf.ProtoMember(8)] public float MovementSpeed;
    /// <summary>
    /// 物资获取倍率
    /// </summary>
    [global::ProtoBuf.ProtoMember(9)] public float ResourceMultiplier;
    /// <summary>
    /// 物资获取倍率触发概率（触发时获取物资倍率为“物资获取倍率，不触发时获取物资倍率为1）
    /// </summary>
    [global::ProtoBuf.ProtoMember(10)] public float ResourceMultiplierProcChance;
    
    public RaceInGameConfig(Dictionary<string, object> dataDic)
    {
        Initialize(dataDic);
    }
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