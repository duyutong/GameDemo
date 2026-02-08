using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas UICanvas { get; private set; }
    public Camera UICamera { get; private set; }
    public Transform MainCanvas { get; private set; }
    private static string prefabPath = "Assets/AddressableAssets/Prefabs/UI";
    private static Dictionary<Type, GameObject> UIWinDic = new();
    public void Init(GameObject uiRootPrefab,Transform parent)
    {
        GameObject uiRoot = Instantiate(uiRootPrefab, Vector3.zero, Quaternion.identity);
        uiRoot.transform.SetParent(parent, false);
        UICanvas = uiRoot.GetComponentInChildren<Canvas>();
        UICamera = uiRoot.GetComponentInChildren<Camera>();
        MainCanvas = UICanvas.transform;
    }
    public void OpenWindow<T>(Action<T> onDone) where T : UIWindowComponentBase
    {
        Type type = typeof(T);
        if (UIWinDic.TryGetValue(type, out GameObject winObj))
        {
            onDone?.Invoke(winObj.GetComponent<T>());
            return;
        }
        else 
        {
            string windowName = typeof(T).Name;
            string fullPath = GetUIWindowPath(windowName);
            ResourceLoader.Load<GameObject>(fullPath, (obj) =>
            {
                if (obj != null)
                {
                    GameObject winIns = Instantiate(obj);
                    winIns.transform.SetParent(MainCanvas, false);
                    winIns.transform.SetAsLastSibling();
                    T win = winIns.GetComponent<T>();
                    win?.Open();
                    UIWinDic[type] = winIns;
                    onDone?.Invoke(win);
                }
            }, false);
        }
        
    }

    private static string GetUIWindowPath(string windowName)
    {
        return $"{prefabPath}/{windowName}/{windowName}.prefab";
    }

    public void CloseWindow<T>() where T : UIWindowComponentBase
    {

    }
}
