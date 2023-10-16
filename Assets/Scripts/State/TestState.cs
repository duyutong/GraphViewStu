using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class TestState : BehaviorTreeBaseState
{
    public Boolean p0;
    public Single p2;

    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<TestStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;

                _stateObj.p0 = p0;
                _stateObj.p2 = p2;
            }
            return _stateObj;
        }
    }
    private TestStateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TestStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (TestStateObj)jsonSerializer.ReadObject(stream);

            p0 = _stateObj.p0;
            p2 = _stateObj.p2;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;

        p0 = _stateObj.p0;
        p2 = _stateObj.p2;
    }
}
public class TestStateObj : ScriptableObject
{
    public EBTState state;

    public Boolean p0;
    public Single p2;

    public List<SBTOutputInfo> output;
}
