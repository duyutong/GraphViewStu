using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[Serializable]
public class BehaviorTreeBaseState
{
    public List<BehaviorTreeBaseState> lastStates = new List<BehaviorTreeBaseState>();
    public EBTState state = EBTState.未开始;
    public string stateName = "*BehaviorTreeBaseState";
    public BTRuntimeComponent runtime;
    public virtual ScriptableObject stateObj { get; }
    public virtual void Init(string param) { }
    public virtual void InitValue()
    {
        Type type = GetType();
        foreach (BehaviorTreeBaseState lastState in lastStates)
        {
            foreach (SBTOutputInfo inputInfo in lastState.output)
            {
                if (inputInfo.value == null) continue;
                string memberName = inputInfo.toPortName;
                FieldInfo fieldInfo = type.GetField(memberName);
                if (fieldInfo == null) continue;
                fieldInfo.SetValue(this, inputInfo.value);
            }
        }
    }
    public void RefreshOutput(SBTOutputInfo newInfo, bool isRemove)
    {
        if (isRemove)
        {
            for (int i = output.Count - 1; i > 0; i--)
            {
                SBTOutputInfo info = output[i];
                if (info.fromPortName != newInfo.fromPortName) continue;
                if (info.toPortName != newInfo.toPortName) continue;
                output.Remove(info);
            }
        }
        else
            output.Add(newInfo);
    }
    public virtual void Save() { }
    public List<SBTOutputInfo> output = new List<SBTOutputInfo>();
    public virtual void OnEnter() { InitValue(); state = EBTState.进入; }

    public virtual void OnExecute() { state = EBTState.执行中; }
    public virtual void OnExit() { state = EBTState.完成; }
    public virtual void OnRefresh() 
    {
        state = EBTState.未开始;
        ClearOutputList();
    }
    public virtual void ClearOutputList() 
    {
        for (int i = 0; i < output.Count; i++) 
        {
            SBTOutputInfo info = output[i];
            info.value = null;
            output[i] = info;
        }
    }
    public virtual void OnUpdate()
    {
        if (state != EBTState.执行中) goto wait;
        wait:;
    }

}
[Serializable]
public enum EBTState
{
    未开始,
    进入,
    执行中,
    完成,
}
[Serializable]
public struct SBTOutputInfo
{
    public string fromPortName;
    public string toPortName;
    public object value;
}
