using UnityEngine;
using UnityEngine.UI;
using static EnumDefinitions;

public class UIPanel_Bag_ButtonGroup : MonoBehaviour
{
    public BTRuntimeComponent bTRuntimeComp;
    public Button btnUse;
    public Button btnSubmit;

    public void InitUI() 
    {
        bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_Open");
    }
    public void SetUseButtonShowOrHide(ItemConfig itemConfig)
    {
        bool isClue = (EItemType)itemConfig.EItemType == EItemType.Clue;
        bool isFood = (EItemType)itemConfig.EItemType == EItemType.Food;
        bool isSurvival = (EItemType)itemConfig.EItemType == EItemType.Survival;

        if (isClue)
        {
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnSubmit_Show");
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnUse_Hide");
        }
        else 
        {
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnSubmit_Hide");
            bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_BtnUse_Show");
        }
    }
}
