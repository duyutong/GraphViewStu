using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DefaultState : BehaviorTreeBaseState
{
    public string stateName => "DefaultState";

    public override BTStateObject stateObj 
    {
        get 
        {
            if (_stateObj == null) _stateObj = new DefaultStateObj();
            return _stateObj;
        }
    }
    private DefaultStateObj _stateObj;
    public override void OnEnter() { }

    public override void OnExecute() { }

    public override void OnExit() { }

    public override void OnUpdate() { }
}
public class DefaultStateObj: BTStateObject
{
    public string text = "DefaultState";
}
