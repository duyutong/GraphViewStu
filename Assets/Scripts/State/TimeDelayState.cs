
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class TimeDelayState : BehaviorTreeBaseState
{
    public Single delayTime;

    private float currTime;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<TimeDelayStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.delayTime = delayTime;
            }
            return _stateObj;
        }
    }
    private TimeDelayStateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TimeDelayStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (TimeDelayStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;
            delayTime = _stateObj.delayTime;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        delayTime = _stateObj.delayTime;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        currTime = 0;
        OnExecute();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        currTime += Time.deltaTime;
        if (currTime >= delayTime) 
        {
            OnExit();
        }
    }
}
public class TimeDelayStateObj : BTStateObject
{
    public EBTState state;

    public Single delayTime;

    public List<BTOutputInfo> output;
}
