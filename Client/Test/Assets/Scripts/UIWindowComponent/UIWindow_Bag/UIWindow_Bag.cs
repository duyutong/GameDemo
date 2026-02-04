using System;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow_Bag : UIWindowComponentBase
{
    public BTRuntimeComponent bTRuntimeComp;
    public GameObject itemPrefab;
    public List<int> testItems;
    public UIPanel_Bag_ItemView itemView;
    public UIPanel_Bag_DescriptionView descriptionView;
    public UIPanel_Bag_ButtonGroup buttonGroup;

    private Action<ItemConfig> action;

    protected override void OnOpen()
    {
        base.OnOpen();
        bTRuntimeComp.SendMsgToBTRuntime("UIWindow_Bag_Open");
        OnInit();
    }
    protected override void OnInit()
    {
        base.OnInit();
        action += descriptionView.SetData;
        action += buttonGroup.SetUseButtonShowOrHide;
        itemView.SetItemBroadcastEvent(action);
        itemView.Initialize(testItems, itemPrefab);

        buttonGroup.InitUI();
    }

    protected override void OnUninit()
    {
        action = null;
        itemView.Uninitialize();
        base.OnUninit();
    }
}
