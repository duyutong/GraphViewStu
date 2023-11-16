using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

public class DebugState : BehaviorTreeBaseState
{
    public string logStr;
    public override BTStateObject stateObj 
    {
        get 
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<DebugStateObj>();
                _stateObj.state = state;
                _stateObj.logStr = logStr;
            }
            return _stateObj;
        }
    }
    private DebugStateObj _stateObj;
    public override void InitParam(string param)
    {
       base.InitParam(param);
       DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DebugStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (DebugStateObj)jsonSerializer.ReadObject(stream);
            logStr = _stateObj.logStr;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        logStr = _stateObj.logStr;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        OnExecute();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        Debug.Log(logStr);
        OnExit();
    }
    public override void OnExit() 
    {
        base .OnExit();
    }
}
public class DebugStateObj : BTStateObject
{
    public EBTState state;
    public string logStr;
}
