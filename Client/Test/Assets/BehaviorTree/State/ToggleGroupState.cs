
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[Serializable]
public class ToggleGroupState : TiggerBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean isAnyTogglesOn;
    public System.Boolean isAllTogglesOff;
    public System.Boolean enter;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ToggleGroupStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.isAnyTogglesOn = isAnyTogglesOn;
                _stateObj.isAllTogglesOff = isAllTogglesOff;
                _stateObj.enter = enter;
            }
            return _stateObj;
        }
    }
    private ToggleGroupStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ToggleGroupStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<ToggleGroupStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;

            exit = _stateObj.exit;
            isAnyTogglesOn = _stateObj.isAnyTogglesOn;
            isAllTogglesOff = _stateObj.isAllTogglesOff;
            enter = _stateObj.enter; triggerTag = _stateObj.triggerTag;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "isAnyTogglesOn") && value is System.Boolean isAnyTogglesOnValue) isAnyTogglesOn = isAnyTogglesOnValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "isAllTogglesOff") && value is System.Boolean isAllTogglesOffValue) isAllTogglesOff = isAllTogglesOffValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        isAnyTogglesOn = _stateObj.isAnyTogglesOn;
        isAllTogglesOff = _stateObj.isAllTogglesOff;
        enter = _stateObj.enter; triggerTag = _stateObj.triggerTag;
    }
    #endregion

    private Toggle toggle;
    private ToggleGroup toggleGroup;
    public override void OnEnter()
    {
        base.OnEnter();
        if (runtime != null)
        {
            if (toggle == null) toggle = runtime.gameObject.GetComponent<Toggle>();
            if (toggleGroup == null && toggle != null) toggleGroup = toggle.group;
        }

        bool isCanExecute = enter && runtime != null && toggleGroup != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        isAnyTogglesOn = toggleGroup.AnyTogglesOn();
        isAllTogglesOff = !isAnyTogglesOn;
        OnExit();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        enter = _stateObj.enter;
        isAnyTogglesOn = false;
        isAllTogglesOff = false;
    }
    public override void ClearOutputList()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            info.value = false;
            output[i] = info;
        }
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "isAnyTogglesOn") info.value = isAnyTogglesOn;
            if (info.fromPortName == "isAllTogglesOff") info.value = isAllTogglesOff;
            if (info.fromPortName == "exit") info.value = enter;
        }

        isAnyTogglesOn = false;
        isAllTogglesOff = false;
        base.OnExit();
    }
}

#region AutoContext_BTStateObject
public class ToggleGroupStateObj : BTTiggerStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean isAnyTogglesOn;
    public System.Boolean isAllTogglesOff;
    public System.Boolean enter;
}
#endregion
