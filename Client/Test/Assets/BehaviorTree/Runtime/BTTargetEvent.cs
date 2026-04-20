using System;
using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class PersistentData
{
    public UnityEngine.Object target;   // 目标 GameObject
    public string assemblyTypeName;     // 类名
    public string methodName;           // 方法名

    public BTTargetObject btTargetObject;
    public void SerializeBTTargetObject()
    {
        if (btTargetObject == null) btTargetObject = new BTTargetObject();
        btTargetObject.target = target;

        bool isPrefabStage = IsPrefabStage();
        btTargetObject.pathType = isPrefabStage ? EFindObjPathType.LocalPath : EFindObjPathType.ScenePath;
        
        btTargetObject.SerializeSelf();
        target = null;
    }
    private bool IsPrefabStage()
    {
        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        return stage != null; // 如果不为空，说明当前是 Prefab 编辑窗口
    }
}

[Serializable]
public class BTTargetEvent
{
    [SerializeField]
    [HideInInspector]
    public PersistentData[] persistentDatas;

    [SerializeField]
    public UnityEvent targetEvent;

    [NonSerialized]
    public BTRuntime runtime;
    public void SetTargetEvent()
    {
        if (targetEvent != null && targetEvent.GetPersistentEventCount() > 0) return;
        if (persistentDatas == null) return;

        targetEvent ??= new UnityEvent();
        targetEvent.RemoveAllListeners();
        for (int i = 0; i < persistentDatas.Length; i++)
        {
            int index = i;
            PersistentData persistentData = persistentDatas[index];
            BTTargetObject btTargetObject = persistentData.btTargetObject;
            if (btTargetObject == null) continue;
           
            btTargetObject.runtime = runtime;
            btTargetObject.SetObejctByPath();
            persistentData.target = btTargetObject.target;
            IntegrateEventInfo(persistentData);
        }
    }
    public void SerializeSelf()
    {
        if (targetEvent != null && targetEvent.GetPersistentEventCount() > 0)
        {
            int persistentEventCount = targetEvent.GetPersistentEventCount();
            persistentDatas = new PersistentData[persistentEventCount];
            for (int i = 0; i < persistentEventCount; i++)
            {
                int index = i;
                PersistentData persistentData = ExtractEventInfo(index);
                persistentData.SerializeBTTargetObject();
                persistentDatas.SetValue(persistentData, index);
            }
        }
        targetEvent?.RemoveAllListeners();
        targetEvent = null;
    }
    private void IntegrateEventInfo(PersistentData data)
    {
        var targetComponent = data.target.GameObject().GetComponent(data.assemblyTypeName);
        if (targetComponent == null) return;

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        var method = targetComponent.GetType().GetMethod(data.methodName, flags);
        if (method == null) return;

        if (Delegate.CreateDelegate(typeof(UnityAction), targetComponent, method) is UnityAction action)
        {
            targetEvent.AddListener(action);
//#if UNITY_EDITOR
//            UnityEventTools.AddPersistentListener(targetEvent, action);
//#else
//            targetEvent.AddListener(action);
//#endif
        }
    }
    private PersistentData ExtractEventInfo(int index)
    {
        if (targetEvent == null) return null;
        if (index < 0 || index >= targetEvent.GetPersistentEventCount()) return null;

        var target = targetEvent.GetPersistentTarget(index);
        if (target == null) return null;

        string assemblyTypeName = target.GetType().FullName;
        var methodName = targetEvent.GetPersistentMethodName(index);
        
        return new PersistentData
        {
            target = target,
            methodName = methodName,
            assemblyTypeName = assemblyTypeName
        };
    }
}
