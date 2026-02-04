using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_Bag_ItemView : MonoBehaviour
{
    private Action<ItemConfig> itemBroadcast;
    private List<RectTransform> slotList = new();
    public void Initialize(List<int> items, GameObject prefab)
    {
        InitSlotList();
        ClearSlots();
        CreateItems(items, prefab);
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
    private void CreateItems(List<int> items, GameObject prefab)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            int itemId = items[index];
            GameObject itemPrefab = Instantiate(prefab, slotList[index]);
            itemPrefab.SetActive(true);
            itemPrefab.transform.Reset();
            itemPrefab.GetComponent<UIItem_Bag_Item>().SetData(itemId);
            itemPrefab.GetComponent<UIItem_Bag_Item>().ItemBroadcast = itemBroadcast;
        }
    }

    private void ClearSlots()
    {
        foreach (RectTransform slot in slotList)
            slot.RemoveChildren();
    }
}
