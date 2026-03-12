
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class DragMoveState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public UnityEngine.Vector2 dragOffset;
    public BTTargetObject targetObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<DragMoveStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.dragOffset = dragOffset;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private DragMoveStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DragMoveStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<DragMoveStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            dragOffset = _stateObj.dragOffset;
            targetObj = _stateObj.targetObj;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "dragOffset") && value is UnityEngine.Vector2 dragOffsetValue) dragOffset = dragOffsetValue;
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

        exit = _stateObj.exit;
        enter = _stateObj.enter;
        dragOffset = _stateObj.dragOffset;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    private RectTransform targetRect;
    private RectTransform parentRect;
    public override void OnEnter()
    {
        base.OnEnter();
        if (targetRect == null) targetRect = targetObj.target.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();

        bool isCanExecute = enter && runtime != null && pointerEventData != null;

        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        OnDrag();
        OnExit();
    }
    private void OnDrag()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        targetRect.anchoredPosition = localPoint + dragOffset;
    }
}

#region AutoContext_BTStateObject
public class DragMoveStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public UnityEngine.Vector2 dragOffset;
    public BTTargetObject targetObj;
}
#endregion
