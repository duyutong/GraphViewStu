using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StartState : BehaviorTreeBaseState
{
    public override ScriptableObject stateObj 
    {
        get 
        {
            if (_stateObj == null) _stateObj = new StartStateObj();
            return _stateObj;
        }
    }
    private StartStateObj _stateObj;
    public override void OnEnter() 
    {
        base.OnEnter();
        OnExecute();
    }

    public override void OnUpdate() 
    {
        if (state == EBTState.执行中 && Input.anyKey) OnExit();
        if (state == EBTState.完成 && Input.anyKeyDown) OnEnter();
    }
}
public class StartStateObj: ScriptableObject
{
    public string text = "StartState";
}
