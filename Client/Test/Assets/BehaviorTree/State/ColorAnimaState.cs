using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorAnimaState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public UnityEngine.Color color;
    public BTTargetAnimaCurve animaCurve;
    public BTTargetObject target;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ColorAnimaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.color = color;
                _stateObj.animaCurve = animaCurve;
                _stateObj.target = target;
            }
            return _stateObj;
        }
    }
    private ColorAnimaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ColorAnimaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<ColorAnimaStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;
            interruptible = _stateObj.interruptible;
            interruptTag = _stateObj.interruptTag;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            color = _stateObj.color;
            animaCurve = _stateObj.animaCurve;
            target = _stateObj.target;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "color") && value is UnityEngine.Color colorValue) color = colorValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaCurve") && value is BTTargetAnimaCurve animaCurveValue) animaCurve = animaCurveValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "target") && value is BTTargetObject targetValue) target = targetValue;
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
        color = _stateObj.color;
        animaCurve = _stateObj.animaCurve;
        target = _stateObj.target;
    }
    #endregion

    private RectTransform targetRect;
    private Graphic graphic;
    private Color startColor;
    private float startTime;
    private float endTime;
    private float timeCount;
    public override void OnEnter()
    {
        base.OnEnter();
        if (targetRect == null) targetRect = target.target.GetComponent<RectTransform>();
        if (graphic == null) graphic = targetRect.GetComponent<Graphic>();
        (startTime, endTime) = GetCurveTimeRange(animaCurve.curve);
        startColor = graphic.color;

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        timeCount = 0;
    }
    public override void OnUpdate()
    {
        if (targetRect == null) return;
        if (state != EBTState.执行中) return;


        timeCount += Time.deltaTime;
        if (timeCount > endTime) { timeCount = 0; OnExit(); return; }

        if (startTime <= timeCount && timeCount <= endTime) 
        {
            float t = animaCurve.curve.Evaluate(timeCount);
            graphic.color = Color.Lerp(startColor, color, t);
        }
            
    }
}
#region AutoContext_BTStateObject
public class ColorAnimaStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public UnityEngine.Color color;
    public BTTargetAnimaCurve animaCurve;
    public BTTargetObject target;
}
#endregion
