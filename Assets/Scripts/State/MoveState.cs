using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
[Serializable]
public class MoveState : BehaviorTreeBaseState
{
    public Vector2 direction;
    public float speed;
    public float moveTime;

    private float _timeCount;
    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = new MoveStateObj();
                _stateObj.state = state;
                _stateObj.direction = direction;
                _stateObj.speed = speed;
                _stateObj.output = output;
                _stateObj.moveTime = moveTime;
            }
            return _stateObj;
        }
    }
    private MoveStateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(MoveStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (MoveStateObj)jsonSerializer.ReadObject(stream);
            direction = _stateObj.direction;
            speed = _stateObj.speed;
            output = _stateObj.output;
            moveTime = _stateObj.moveTime;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        direction = _stateObj.direction;
        speed = _stateObj.speed;
    }
    public override void OnEnter()
    {
        base.OnEnter();
        _timeCount = moveTime;
        OnExecute();
    }

    public override void OnUpdate()
    {
        if (runtime == null) return;
        if (state != EBTState.Ö´ÐÐÖÐ) return;

        if (_timeCount <= 0)
        {
            OnExit();
            return;
        }
        else
        {
            _timeCount -= Time.deltaTime;
            runtime.transform.Translate(speed * direction * Time.deltaTime);
        }
    }
}
public class MoveStateObj : ScriptableObject
{
    public EBTState state;
    public Vector2 direction;
    public float speed;
    public float moveTime;
    public List<SBTOutputInfo> output;
}

