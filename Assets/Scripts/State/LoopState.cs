using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class LoopState : BehaviorTreeBaseState
{
    public ELoopState loopState = ELoopState.循环开始;
    public int loopCount = 0;

    private LoopState lastLoopStartState;
    private int currCount;
    public override ScriptableObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = new LoopStateObj();
                _stateObj.loopState = loopState;
                _stateObj.loopCount = loopCount;
                _stateObj.state = state;
            }
            return _stateObj;
        }
    }
    private LoopStateObj _stateObj;

    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LoopStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (LoopStateObj)jsonSerializer.ReadObject(stream);
            loopState = _stateObj.loopState;
            loopCount = _stateObj.loopCount;
            currCount = _stateObj.loopCount;
        }
    }
    public override void OnInitFinish()
    {
        base.OnInitFinish();
        FindLastLoopStart();
    }
    public override void Save()
    {
        if (stateObj == null) return;
        loopState = _stateObj.loopState;
        loopCount = _stateObj.loopCount;
    }
    public override void OnEnter()
    {
        if (loopState == ELoopState.循环开始) OnExit();
        else
        {
            --currCount;
            if (currCount == 0) { OnExit(); return; }
            base.OnExecute();
            Infect((_s) =>
            {
                _s.OnRefresh();
            }, (_s) =>
            {
                if (_s.stateName != "LoopState") return false;
                if ((_s as LoopState).loopState != ELoopState.循环开始) return false;
                return true;

            });

            lastLoopStartState.OnEnter();
        }

    }
    private void FindLastLoopStart()
    {
        lastLoopStartState = GetLastCoupleState<LoopState>((_s) =>
        {
            return _s.loopState == ELoopState.循环开始;
        });
    }
}

public class LoopStateObj : ScriptableObject
{
    public EBTState state;
    public ELoopState loopState;
    public int loopCount;
}

[Serializable]
public enum ELoopState
{
    循环开始,
    循环结束,
}
