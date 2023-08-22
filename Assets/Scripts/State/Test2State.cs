
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class Test2State : BehaviorTreeBaseState
{
    public Boolean p1;
    public Vector2 p2;

    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<Test2StateObj>();
                _stateObj.state = state;
                _stateObj.output = output;

                _stateObj.p1 = p1;
                _stateObj.p2 = p2;
            }
            return _stateObj;
        }
    }
    private Test2StateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Test2StateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (Test2StateObj)jsonSerializer.ReadObject(stream);

            p1 = _stateObj.p1;
            p2 = _stateObj.p2;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;

        p1 = _stateObj.p1;
        p2 = _stateObj.p2;
    }
}
public class Test2StateObj : ScriptableObject
{
    public EBTState state;

    public Boolean p1;
    public Vector2 p2;

    public List<SBTOutputInfo> output;
}
