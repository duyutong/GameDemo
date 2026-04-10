using Unity.VisualScripting;
using UnityEngine;

public class GameFramework : MonoBehaviour
{
    public GameObject UIRootPrefab;
    public static UIManager UIMgr { get; private set; }
    public bool IsInited { get; private set; } = false;
    public void Awake()
    {
        Init();
    }
    public void Start()
    {
       
    }
    public void Init()
    {
        if (IsInited) return;

        if (UIMgr == null) UIMgr = new UIManager();
        UIMgr.Init(UIRootPrefab, transform);

        IsInited = true;
    }
}
