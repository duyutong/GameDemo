
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class ScaleAxisLockState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetAnimaCurve animaCurve;
    public BTTargetObject target;
    public UnityEngine.Vector3 lockAxis;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ScaleAxisLockStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.animaCurve = animaCurve;
                _stateObj.target = target;
                _stateObj.lockAxis = lockAxis;
            }
            return _stateObj;
        }
    }
    private ScaleAxisLockStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ScaleAxisLockStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<ScaleAxisLockStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            animaCurve = _stateObj.animaCurve;
            target = _stateObj.target;
            lockAxis = _stateObj.lockAxis;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaCurve") && value is BTTargetAnimaCurve animaCurveValue) animaCurve = animaCurveValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "target") && value is BTTargetObject targetValue) target = targetValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "lockAxis") && value is UnityEngine.Vector3 lockAxisValue) lockAxis = lockAxisValue;
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
        animaCurve = _stateObj.animaCurve;
        target = _stateObj.target;
        lockAxis = _stateObj.lockAxis;
    }
    #endregion

    private float startTime;
    private float endTime;
    private float timeCount;
    private Transform targetTrans;
    public override void OnEnter()
    {
        base.OnEnter();

        if (targetTrans == null)
        {
            Transform transform = target.target as Transform;
            if (transform != null) targetTrans = transform;
            else { OnRefresh(); return; }
        }

        (startTime, endTime) = GetCurveTimeRange(animaCurve.curve);

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnRefresh()
    {
        timeCount = 0;
       
        base.OnRefresh();
    }
    public override void OnRecycle()
    {
        timeCount = 0;
        targetTrans = null;
        base.OnRecycle();
    }
    public override void OnUpdate()
    {
        if (runtime == null) return;
        if (state != EBTState.Ö´ÐÐÖÐ) return;

        timeCount += Time.deltaTime;
        if (startTime <= timeCount)
        {
            float scaleRatio = animaCurve.curve.Evaluate(Mathf.Clamp(timeCount,startTime, endTime));
            Vector3 result = lockAxis * scaleRatio;
            if (lockAxis.x == 0) result.x = 1;
            if (lockAxis.y == 0) result.y = 1;
            if (lockAxis.z == 0) result.z = 1;
            targetTrans.localScale = result;

            if (timeCount > endTime) { OnExit(); return; }
        }
    }
}

#region AutoContext_BTStateObject
public class ScaleAxisLockStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetAnimaCurve animaCurve;
    public BTTargetObject target;
    public UnityEngine.Vector3 lockAxis;
}
#endregion
