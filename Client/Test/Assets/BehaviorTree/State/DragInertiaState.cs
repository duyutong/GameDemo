using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class DragInertiaState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public System.Single speed;
    public System.Single AxisOffset_Z;
    public BTTargetObject targetObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<DragInertiaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.speed = speed;
                _stateObj.AxisOffset_Z = AxisOffset_Z;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private DragInertiaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DragInertiaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<DragInertiaStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            speed = _stateObj.speed;
            AxisOffset_Z = _stateObj.AxisOffset_Z;
            targetObj = _stateObj.targetObj;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "speed") && value is System.Single speedValue) speed = speedValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "AxisOffset_Z") && value is System.Single AxisOffset_ZValue) AxisOffset_Z = AxisOffset_ZValue;
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
        speed = _stateObj.speed;
        AxisOffset_Z = _stateObj.AxisOffset_Z;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    private Vector2 lastPos;

    private RectTransform parentRect;
    private RectTransform targetRect;
    private RectTransform canvasRect;
    private Canvas canvas;
    public override void OnCheck()
    {
        base.OnCheck();

        if (canvas == null) canvas = GameObject.FindFirstObjectByType<Canvas>();
        if (targetRect == null) targetRect = targetObj.target.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();
        if (canvasRect == null) canvasRect = canvas.GetComponent<RectTransform>();

        if (state != EBTState.执行中) RefreshRotation();
    }
    public override void OnEnter()
    {
        base.OnEnter();

        bool isCanExecute = enter && runtime != null && pointerEventData != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        base.OnExecute();

        if (runtime == null) return;
        if (state != EBTState.执行中) return;

        OnBeginDrag();
        OnDrag();

        OnExit();
    }
    public override void OnExit()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        lastPos = localPoint;
        base.OnExit();
    }
    private void OnDrag()
    {
        // 将屏幕坐标转换为 Canvas 空间坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 posInSelf);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        Vector2 delta = localPoint - lastPos;
        //设置惯性效果
        Vector3 currAngles = ResetRotation();
        int direction = posInSelf.y > 0 ? -1 : 1;
        float targetZ = Mathf.Lerp(currAngles.z, direction * delta.x, speed * Time.deltaTime);
        targetZ = Math.Clamp(targetZ, -1 * AxisOffset_Z, AxisOffset_Z);
        targetRect.localEulerAngles = targetZ * Vector3.forward;
    }
    private void OnBeginDrag()
    {
        if (lastPos != Vector2.zero) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        lastPos = localPoint;
    }
    public void RefreshRotation()
    {
        Vector3 currAngles = ResetRotation();
        float targetZ = Mathf.Lerp(currAngles.z, 0, speed * Time.deltaTime);
        float deltaZ = targetZ - currAngles.z;

        Vector3 deltaVec3 = deltaZ * Vector3.forward;
        targetRect.localEulerAngles += deltaVec3;
    }
    private Vector3 ResetRotation()
    {
        Vector3 currAngles = targetRect.localEulerAngles;
        while (currAngles.z > AxisOffset_Z) currAngles.z -= 360;
        while (currAngles.z < -AxisOffset_Z) currAngles.z += 360;
        targetRect.localEulerAngles = currAngles;
        return currAngles;
    }
}
#region AutoContext_BTStateObject
public class DragInertiaStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public System.Single speed;
    public System.Single AxisOffset_Z;
    public BTTargetObject targetObj;
}
#endregion