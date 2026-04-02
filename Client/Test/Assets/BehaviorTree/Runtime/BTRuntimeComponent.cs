using UnityEngine;

public class BTRuntimeComponent : MonoBehaviour
{
    public BTContainer container;
    public BTRuntime runtime;
    public bool IsEnable { get; private set; }
    public void OnEnable()
    {
        InitRuntime();
        runtime?.OnEnable();
    }
   
    public void SetEnable(bool value)
    {
        if (IsEnable == value) return;
        IsEnable = value;
        if (IsEnable) runtime?.OnEnable();
        else runtime?.OnDisable();
    }
    public void InitRuntime()
    {
        runtime ??= new BTRuntime();
        runtime.container = container;
        runtime.gameObject = gameObject;
        runtime.transform = transform;
    }

    private void Update()
    {
        runtime?.OnUpdate();
    }
    public void OnDisable()
    {
        runtime?.OnDisable();
    }
    public void OnDestroy()
    {
        runtime?.OnDestroy();
    }
    public void SendMsgToBTRuntime(string triggerTag, EBTState state = EBTState.进入)
    {
        runtime?.OnReceiveMsg(triggerTag, state);
    }
}
