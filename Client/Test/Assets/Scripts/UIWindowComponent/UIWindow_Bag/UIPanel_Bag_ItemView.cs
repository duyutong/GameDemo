using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_Bag_ItemView : MonoBehaviour
{
    private GameObject itemPrefab;
    private Action<ItemConfig> itemBroadcast;
    private List<RectTransform> slotList = new();
    private List<int> currItems = new();
    public void Initialize(GameObject prefab)
    {
        itemPrefab = prefab;
        currItems.Clear();
        InitSlotList();
        ClearSlots();
        RefreshItems();
    }
    public void AddItems(int itemId)
    {
        currItems.Add(itemId);
        RefreshItems();
    }
    public void RemoveItems(int itemId)
    {
        currItems.Remove(itemId);
        ClearSlots();
        RefreshItems();
    }
    public void Refresh(List<int> items)
    {
        currItems = items;
        ClearSlots();
        RefreshItems();
    }
    public void SetItemBroadcastEvent(Action<ItemConfig> action) 
    {
        itemBroadcast = action;
    }
    public void Uninitialize()
    {
        itemBroadcast = null;
        ClearSlots();
    }
    private void InitSlotList()
    {
        if (slotList.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform slot = transform.GetChild(i);
                slotList.Add(slot.GetComponent<RectTransform>());
            }
        }
    }
    private void RefreshItems()
    {
        for (int i = 0; i < currItems.Count; i++)
        {
            int index = i;
            int itemId = currItems[index];
            if (slotList[index].childCount > 0) continue;
            
            GameObject itemObj = Instantiate(itemPrefab, slotList[index]);
            itemObj.SetActive(true);
            itemObj.transform.Reset();
            itemObj.GetComponent<UIItem_Bag_Item>().SetData(itemId);
            itemObj.GetComponent<UIItem_Bag_Item>().ItemBroadcast = itemBroadcast;
        }
    }

    private void ClearSlots()
    {
        foreach (RectTransform slot in slotList)
            slot.RemoveChildren();
    }
}
