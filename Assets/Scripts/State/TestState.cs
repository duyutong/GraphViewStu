using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class TestState : BehaviorTreeBaseState
{
    public Boolean enter;
    public Vector3 dir;
    public Single speed;

    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<TestStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;

                _stateObj.enter = enter;
                _stateObj.dir = dir;
                _stateObj.speed = speed;
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

            enter = _stateObj.enter;
            dir = _stateObj.dir;
            speed = _stateObj.speed;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;

        enter = _stateObj.enter;
        dir = _stateObj.dir;
        speed = _stateObj.speed;
    }
}
public class TestStateObj : ScriptableObject
{
    public EBTState state;

    public Boolean enter;
    public Vector3 dir;
    public Single speed;

    public List<SBTOutputInfo> output;
}
