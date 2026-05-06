using UnityEngine;

public class PopupPanelUI : MonoBehaviour
{
    public GameObject panel;

    // 打开
    public void OpenPanel()
    {
        panel.SetActive(true);
        Debug.Log("Panel Opened");
    }

    // 关闭
    public void ClosePanel()
    {
        panel.SetActive(false);
        Debug.Log("Panel Closed");
    }

    // 四个按钮测试
    public void OnClickButton1()
    {
        Debug.Log("Clicked Button 1 - Box");
    }

    public void OnClickButton2()
    {
        Debug.Log("Clicked Button 2 - Search");
    }

    public void OnClickButton3()
    {
        Debug.Log("Clicked Button 3 - Swap");
    }

    public void OnClickButton4()
    {
        Debug.Log("Clicked Button 4 - Arrow");
    }
}
