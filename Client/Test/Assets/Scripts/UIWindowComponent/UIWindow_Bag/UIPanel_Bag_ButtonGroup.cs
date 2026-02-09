using Network.API;
using UnityEngine;
using UnityEngine.UI;
using static EnumDefinitions;

public class UIPanel_Bag_ButtonGroup : MonoBehaviour
{
    public BTRuntimeComponent bTRuntimeComp;
    public Button btnUse;
    public Button btnSubmit;
    public Button btnIdentified;

    private ItemConfig currConfig;
    
    private BagApi bagApi=>ApiManager.GetHttpApi<BagApi>();
    public void InitUI() 
    {
        bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_Open");
    }
    public void SetUseButtonShowOrHide(ItemConfig itemConfig)
    {
        currConfig = itemConfig;

        bool isClue = (EItemType)itemConfig.EItemType == EItemType.Clue;
        bool isFood = (EItemType)itemConfig.EItemType == EItemType.Food;
        bool isSurvival = (EItemType)itemConfig.EItemType == EItemType.Survival;
        bool isUnidentified = (EItemType)itemConfig.EItemType == EItemType.Unidentified;

        bTRuntimeComp.SendMsgToBTRuntime("Interrupt_bag_buttonGroup",EBTState.中断);
        
        if (isUnidentified)
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnIdentified_Show");
        else if (isClue)
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnSubmit_Show");
        else
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnUse_Show");
    }
    public void OnClickBtnIdentify()
    {
        if (currConfig == null) return;
        Debug.Log($"{currConfig.ID}");
    }
}
