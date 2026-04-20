using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIDragBranchingState : UIEventBranchingState
{
    #region AutoContext

    public System.Boolean beginDrag;
    public System.Boolean drag;
    public System.Boolean endDrag;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<UIDragBranchingStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.beginDrag = beginDrag;
                _stateObj.drag = drag;
                _stateObj.endDrag = endDrag;
                _stateObj.idel = idel;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private UIDragBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(UIDragBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<UIDragBranchingStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            beginDrag = _stateObj.beginDrag;
            drag = _stateObj.drag;
            endDrag = _stateObj.endDrag;
            idel = _stateObj.idel;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "beginDrag") && value is System.Boolean beginDragValue) beginDrag = beginDragValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "drag") && value is System.Boolean dragValue) drag = dragValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "endDrag") && value is System.Boolean endDragValue) endDrag = endDragValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "idel") && value is System.Boolean idelValue) idel = idelValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "targetObj") && value is BTTargetObject targetObjValue) targetObj = targetObjValue;
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

        beginDrag = _stateObj.beginDrag;
        drag = _stateObj.drag;
        endDrag = _stateObj.endDrag;
        idel = _stateObj.idel;
        enter = _stateObj.enter;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    private RectTransform rectTransform;
    private bool isInitFinish;
    public override void OnEnter()
    {
        base.OnEnter();

        if (rectTransform == null && targetObj != null) rectTransform = targetObj.target.GetComponent<RectTransform>();

        if (runtime != null && !isInitFinish)
        {
            isInitFinish = true;
            OnSetPointTriggerEvent(targetObj);

            trigger.AddTriggerEventListener(EventTriggerType.BeginDrag, OnMyBeginDrag);
            trigger.AddTriggerEventListener(EventTriggerType.Drag, OnMyDrag);
            trigger.AddTriggerEventListener(EventTriggerType.EndDrag, OnMyEndDrag);
            trigger.AddTriggerEventListener(EventTriggerType.PointerUp, OnPointerUp);
        }
    }
    public override void OnRefresh()
    {
        base.OnRefresh();

        if (drag) return;

        beginDrag = _stateObj.beginDrag;
        drag = _stateObj.drag;
        endDrag = _stateObj.endDrag;
        idel = _stateObj.idel;
    }
    public override void OnExit()
    {
        bool hasBeginDrag = false;
        bool hasDrag = false;
        bool hasEndDrag = false;
        bool hasIdel = false;

        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "beginDrag") { info.value = beginDrag; hasBeginDrag = true; }
            if (info.fromPortName == "drag") { info.value = drag; hasDrag = true; }
            if (info.fromPortName == "endDrag") { info.value = endDrag; hasEndDrag = true; }
            if (info.fromPortName == "idel") { info.value = idel; hasIdel = true; }
            output[i] = info;
        }

        bool canExit = (beginDrag && hasBeginDrag) || (drag && hasDrag) || (endDrag && hasEndDrag) || (idel && hasIdel);

        if (canExit) base.OnExit();
        else OnRefresh();
    }
    private void OnPointerUp(PointerEventData data)
    {
        idel = true;
        drag = false;
        beginDrag = false;
        endDrag = false;

        OnExit();
    }
    private void OnMyDrag(PointerEventData data)
    {
        if (beginDrag) { beginDrag = false; return; }

        drag = true;
        beginDrag = false;
        endDrag = false;
        idel = false;

        OnExit();
    }

    private void OnMyEndDrag(PointerEventData data)
    {
        drag = false;
        beginDrag = false;
        endDrag = true;
        idel = false;

        OnExit();
    }

    private void OnMyBeginDrag(PointerEventData data)
    {
        drag = false;
        beginDrag = true;
        endDrag = false;
        idel = false;

        OnExit();
    }
}
#region AutoContext_BTStateObject
public class UIDragBranchingStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean beginDrag;
    public System.Boolean drag;
    public System.Boolean endDrag;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;
}
#endregion