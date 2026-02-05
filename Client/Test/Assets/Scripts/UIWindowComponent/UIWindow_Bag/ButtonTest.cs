using Network.API;
using Network.Models;
using System;
using UnityEngine;

public class ButtonTest : MonoBehaviour
{
    public UIWindow_Bag uIWindow_Bag;
    private UIPanel_Bag_ItemView itemView;
    private BagApi bagApi => ApiManager.GetHttpApi<BagApi>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickTest()
    {
        if (itemView == null) itemView = uIWindow_Bag.itemView;
        BagAcquireItemRequest request = new BagAcquireItemRequest();
        bagApi.BagAcquireItem(request, OnGetItem);
    }

    private void OnGetItem(bool succ, BagAcquireItemResponse response)
    {
        if (!succ) return;
        itemView.AddItems(response.Item.Id);
    }
}
