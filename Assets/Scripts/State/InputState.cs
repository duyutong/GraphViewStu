using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

[Serializable]
public class InputState : BehaviorTreeBaseState
{
    public Vector2 direction;
    private bool isInput { get { return direction != Vector2.zero; } }
    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = new InputStateObj();
                _stateObj.state = state;
                _stateObj.direction = direction;
                _stateObj.output = output;
            }
            return _stateObj;
        }
    }
    private InputStateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(InputStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (InputStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;
        }

    }
    public override void OnEnter()
    {
        state = EBTState.执行中;
        direction = Vector2.zero;
    }
    public override void OnUpdate()
    {
        if (Input.anyKey) OnExecute();
        if (isInput && !Input.anyKey) OnPointerUp();
    }
    public override void OnExecute()
    {
        if (Input.GetKey(KeyCode.W)) direction = new Vector2(direction.x, 1);
        if (Input.GetKey(KeyCode.A)) direction = new Vector2(-1, direction.y);
        if (Input.GetKey(KeyCode.S)) direction = new Vector2(direction.x, -1);
        if (Input.GetKey(KeyCode.D)) direction = new Vector2(1, direction.y);

    }
    public override void OnExit()
    {
        state = EBTState.完成;
    }
    private void OnPointerUp()
    {
        for (int i = 0; i < output.Count; i++)
        {
            SBTOutputInfo info = output[i];
            if (info.fromPortName == "result"
                && ((info.value != null && (Vector2)info.value != direction) || info.value == null))
            {
                info.value = direction;
                output[i] = info;
            }
        }
        OnExit();
    }
}
public class InputStateObj : ScriptableObject
{
    public EBTState state;
    public Vector2 direction;
    public List<SBTOutputInfo> output;
}
