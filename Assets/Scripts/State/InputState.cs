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
    public override BTStateObject stateObj 
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<InputStateObj>();
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
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(InputStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (InputStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;
        }

    }
    public override void OnEnter()
    {
        direction = Vector2.zero;
        OnExecute();
    }
    public override void OnUpdate()
    {
        if (Input.anyKey) OnInputKey();
        if (isInput && !Input.anyKey) OnPointerUp();
    }
    private void OnInputKey()
    {
        if (Input.GetKey(KeyCode.W)) direction = new Vector2(direction.x, 1);
        if (Input.GetKey(KeyCode.A)) direction = new Vector2(-1, direction.y);
        if (Input.GetKey(KeyCode.S)) direction = new Vector2(direction.x, -1);
        if (Input.GetKey(KeyCode.D)) direction = new Vector2(1, direction.y);

    }
    private void OnPointerUp()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
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
public class InputStateObj : BTStateObject
{
    public EBTState state;
    public Vector2 direction;
    public List<BTOutputInfo> output;
}
