using ConfigData;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIItem_Bag_Item : MonoBehaviour
{
    public Image imgIcon;
    public Toggle toggle;
    public Action<ItemConfig> ItemBroadcast;

    private int itemId;
    private ItemConfig config;
    private RectTransform selfRectTrans;
    public void SetData(int _id) 
    {
        itemId = _id;
        config = ConfigLoader.GetConfigData<ItemConfig>(itemId);
        ItemBroadcast?.Invoke(config);
        SetIcon(config.Icon);
    }
    public void OnToggleValueChanged() 
    {
        if (toggle == null) return;
        InvokItemBroadcastEvent(toggle.isOn);
    }
    private void InvokItemBroadcastEvent(bool isOn)
    {
        if (!isOn) return;
        ItemBroadcast?.Invoke(config);
    }
    private void SetIcon(string spriteName)
    {
        selfRectTrans??= GetComponent<RectTransform>();
        ResourceLoader.Load<Sprite>(spriteName, (sprite) =>
        {
            imgIcon.sprite = sprite;
            imgIcon.FitImageToRect(selfRectTrans);
        },true);
    }
    private void OnDestroy()
    {
        if (imgIcon.sprite == null) return;
        ResourceLoader.Release(config.Icon, imgIcon.sprite);
    }
}
