
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class bTTestState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean port2;
    public System.Boolean port1;
    public System.Boolean port3;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<bTTestStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.port2 = port2;
                _stateObj.port1 = port1;
                _stateObj.port3 = port3;
            }
            return _stateObj;
        }
    }
    private bTTestStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(bTTestStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<bTTestStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            port2 = _stateObj.port2;
            port1 = _stateObj.port1;
            port3 = _stateObj.port3;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "port2") && value is System.Boolean port2Value) port2 = port2Value;
        else if (StringComparer.Ordinal.Equals(fieldName, "port1") && value is System.Boolean port1Value) port1 = port1Value;
        else if (StringComparer.Ordinal.Equals(fieldName, "port3") && value is System.Boolean port3Value) port3 = port3Value;
        else if (StringComparer.Ordinal.Equals(fieldName, "pointerEventData") && value is PointerEventData PointerEventDataValue) pointerEventData = PointerEventDataValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        port2 = _stateObj.port2;
        port1 = _stateObj.port1;
        port3 = _stateObj.port3;
    }
    #endregion

    public override void OnEnter()
    {
        base.OnEnter();
    }
}

#region AutoContext_BTStateObject
public class bTTestStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean port2;
    public System.Boolean port1;
    public System.Boolean port3;
}
#endregion
