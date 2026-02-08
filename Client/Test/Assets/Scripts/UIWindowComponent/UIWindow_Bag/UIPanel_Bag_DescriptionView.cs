using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Bag_DescriptionView : MonoBehaviour
{
    public Image imgIcon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtDes;

    private ItemConfig config;
    private RectTransform selfRectTrans;
    public void SetData(ItemConfig _config)
    {
        config = _config;
        SetIcon(config.Icon);

        txtName.text = ConfigLoaderUtil.GetLanguageById(config.ItemName);
        txtDes.text = ConfigLoaderUtil.GetLanguageById(config.ItemDesc);
    }
    private void SetIcon(string spriteName)
    {
        if (selfRectTrans == null) selfRectTrans = GetComponent<RectTransform>();
        ResourceLoader.Load<Sprite>(spriteName, (sprite) =>
        {
            imgIcon.sprite = sprite;
            imgIcon.FitImageToRect(selfRectTrans);
        }, true);
    }
}
