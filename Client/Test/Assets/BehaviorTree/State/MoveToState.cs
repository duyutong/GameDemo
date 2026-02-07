
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class MoveToState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public BTTargetObject directionObj;
    public BTTargetAnimaCurve animaCurve;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<MoveToStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
                _stateObj.directionObj = directionObj;
                _stateObj.animaCurve = animaCurve;
            }
            return _stateObj;
        }
    }
    private MoveToStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MoveToStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<MoveToStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
            directionObj = _stateObj.directionObj;
            animaCurve = _stateObj.animaCurve;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "targetObj") && value is BTTargetObject targetObjValue) targetObj = targetObjValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "directionObj") && value is BTTargetObject directionObjValue) directionObj = directionObjValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaCurve") && value is BTTargetAnimaCurve animaCurveValue) animaCurve = animaCurveValue;
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
        targetObj = _stateObj.targetObj;
        directionObj = _stateObj.directionObj;
        animaCurve = _stateObj.animaCurve;
    }
    #endregion

    private float startTime;
    private float endTime;
    private float timeCount;
    private Vector3 startPos;
    private Vector3 targetPos;
    private Transform targetTrans;
    public override void OnEnter()
    {
        base.OnEnter();
        (startTime, endTime) = GetCurveTimeRange(animaCurve.curve);
        if (targetTrans == null) targetTrans = targetObj.target.GetComponent<Transform>();

        if (directionObj.target == null) Debug.Log(directionObj.localPath);
        targetPos = directionObj.target.GetComponent<Transform>().position;
        startPos = targetTrans.position;

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        if (startTime + endTime == 0) { targetTrans.position = targetPos; OnExit(); }
        else base.OnExecute();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        timeCount = 0;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (runtime == null) return;

        timeCount += Time.deltaTime;
        if (timeCount > endTime) { OnExit(); return; }

        if (startTime <= timeCount && timeCount <= endTime)
        {
            float t = animaCurve.curve.Evaluate(timeCount);
            targetTrans.position = Vector3.Lerp(startPos, targetPos, t);

            startPos = targetTrans.position;
        }

    }
}

#region AutoContext_BTStateObject
public class MoveToStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public BTTargetObject directionObj;
    public BTTargetAnimaCurve animaCurve;
}
#endregion
