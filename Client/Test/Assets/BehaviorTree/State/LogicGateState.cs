
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

[Serializable]
public class LogicGateState : BehaviorTreeBaseState
{
    public ELogic logicType;
    public System.Boolean enter;
    public System.Boolean exit;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<LogicGateStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;
                _stateObj.enter = enter;
                _stateObj.exit = exit;

                _stateObj.logicType = logicType;
            }
            return _stateObj;
        }
    }
    private LogicGateStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LogicGateStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = ScriptableObject.CreateInstance<LogicGateStateObj>();
            var json = new StreamReader(stream).ReadToEnd();
            JsonUtility.FromJsonOverwrite(json, _stateObj);

            output = _stateObj.output;

            logicType = _stateObj.logicType;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        logicType = _stateObj.logicType;
    }
    public override void OnInitFinish()
    {
        if (logicType == ELogic.NOT) OnExit();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        if (logicType == ELogic.OR)
        {
            OnExit();
        }
        else if (logicType == ELogic.AND)
        {
            int checkCount = lastStates.Count;
            foreach (BehaviorTreeBaseState lastState in lastStates)
            {
                if (lastState.state == EBTState.完成) checkCount--;
                if (checkCount == 0)
                {
                    OnExit();
                    return;
                }
            }
            OnRefresh();
        }
        else if (logicType == ELogic.NOT)
        {
            OnRefresh();
        }
    }
    public override void OnExit()
    {
        foreach (BTOutputInfo info in output) if (info.fromPortName == "exit") info.value = true;
        base.OnExit();
       
    }
}
public class LogicGateStateObj : BTStateObject
{
    public EBTState state;
    public System.Boolean enter;
    public System.Boolean exit;
    public ELogic logicType;
}
public enum ELogic
{
    AND,
    OR,
    NOT
}
