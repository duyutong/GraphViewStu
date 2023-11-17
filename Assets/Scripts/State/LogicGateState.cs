
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

[Serializable]
public class LogicGateState : BehaviorTreeBaseState
{
    public ELogic logicType;

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
            _stateObj = (LogicGateStateObj)jsonSerializer.ReadObject(stream);
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
        if (logicType == ELogic.NOT) {base.OnExit();}
    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (logicType == ELogic.OR) OnExit();
        else if (logicType == ELogic.AND)
        {
            int checkCount = lastStates.Count;
            foreach (BehaviorTreeBaseState lastState in lastStates)
            {
                if (lastState.state == EBTState.Íê³É) checkCount--;
                if (checkCount == 0) { OnExit(); return; }
            }
            OnRefresh();
        }
        else if(logicType == ELogic.NOT)
        {
            OnRefresh();
        }
    }
}
public class LogicGateStateObj : BTStateObject
{
    public EBTState state;
    public ELogic logicType;
    public List<BTOutputInfo> output;
}
public enum ELogic 
{
    AND,
    OR,
    NOT
}
