
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class SpriteRendererAlphaState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject target;
    public BTTargetAnimaCurve animaCurve;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SpriteRendererAlphaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.target = target;
                _stateObj.animaCurve = animaCurve;
            }
            return _stateObj;
        }
    }
    private SpriteRendererAlphaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SpriteRendererAlphaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<SpriteRendererAlphaStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            target = _stateObj.target;
            animaCurve = _stateObj.animaCurve;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "target") && value is BTTargetObject targetValue) target = targetValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaCurve") && value is BTTargetAnimaCurve animaCurveValue) animaCurve = animaCurveValue;
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
        target = _stateObj.target;
        animaCurve = _stateObj.animaCurve;
    }
    #endregion

    private SpriteRenderer sr;
    private float startTime;
    private float endTime;
    private float timeCount;
    public override void OnEnter()
    {
        base.OnEnter();
        if (sr == null) sr = target.target.GetComponent<SpriteRenderer>();
        (startTime, endTime) = GetCurveTimeRange(animaCurve.curve);

        bool isCanExecute = enter && runtime != null && sr != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        timeCount = 0;
    }
    public override void OnRecycle() 
    {
        timeCount = 0;
        sr = null;
        base.OnRecycle();
    }
    public override void OnUpdate()
    {
        if (sr == null) return;
        if (state != EBTState.Ö´ÐÐÖÐ) return;

        timeCount += Time.deltaTime;
        if (startTime <= timeCount)
        {
            float t = animaCurve.curve.Evaluate(timeCount);
            Color c = sr.color;
            c.a = t;
            sr.color = c;
            if (timeCount > endTime) { OnExit(); return; }
        }
    }
}

#region AutoContext_BTStateObject
public class SpriteRendererAlphaStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject target;
    public BTTargetAnimaCurve animaCurve;
}
#endregion
