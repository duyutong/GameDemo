
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
[Serializable]
public class GetDragOffsetState : BehaviorTreeBaseState
{
    #region AutoContext

    public UnityEngine.Vector2 dragOffset;
    public System.Boolean exit;
    public System.Boolean onDragBegin;
    public System.Boolean onDrag;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<GetDragOffsetStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.dragOffset = dragOffset;
                _stateObj.exit = exit;
                _stateObj.onDragBegin = onDragBegin;
                _stateObj.onDrag = onDrag;
            }
            return _stateObj;
        }
    }
    private GetDragOffsetStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GetDragOffsetStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<GetDragOffsetStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            dragOffset = _stateObj.dragOffset;
            exit = _stateObj.exit;
            onDragBegin = _stateObj.onDragBegin;
            onDrag = _stateObj.onDrag;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "dragOffset") && value is UnityEngine.Vector2 dragOffsetValue) dragOffset = dragOffsetValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "onDragBegin") && value is System.Boolean onDragBeginValue) onDragBegin = onDragBeginValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "onDrag") && value is System.Boolean onDragValue) onDrag = onDragValue;
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

        dragOffset = _stateObj.dragOffset;
        exit = _stateObj.exit;
        onDragBegin = _stateObj.onDragBegin;
        onDrag = _stateObj.onDrag;
    }
    #endregion

    private PointerEventData pointerEventData;
    private RectTransform targetRect;
    private RectTransform parentRect;
    private Vector2 currOffset;
    public override void OnEnter()
    {
        base.OnEnter();

        if (targetRect == null) targetRect = runtime.transform.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();

        bool isCanExecute = (onDragBegin || onDrag) && runtime != null && pointerEventData != null;

        if (isCanExecute) OnExecute();
        else OnExit();
    }
    public override void OnExecute() 
    {
        base.OnExecute();
        if (onDragBegin)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
            dragOffset = targetRect.anchoredPosition - localPoint;
            currOffset = dragOffset;
        }
        else if (onDrag) 
        {
            dragOffset = currOffset;
        }
        OnExit();
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++) 
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "exit") info.value = true;
            if(info.fromPortName == "dragOffset") info.value = dragOffset;
        }
        base.OnExit();
    }
}

#region AutoContext_BTStateObject
public class GetDragOffsetStateObj : BTStateObject
{
    public EBTState state;

    public UnityEngine.Vector2 dragOffset;
    public System.Boolean exit;
    public System.Boolean onDragBegin;
    public System.Boolean onDrag;
}
#endregion
