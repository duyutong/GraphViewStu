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
    public EBTState state = EBTState.δ��ʼ;
    public string stateName = "*BehaviorTreeBaseState";
    public string nodeId;
    public BTRuntimeComponent runtime;
    public List<SBTOutputInfo> output = new List<SBTOutputInfo>();
    public List<BehaviorTreeBaseState> lastStates;
    public virtual ScriptableObject stateObj { get; }
   
    public virtual void InitParam(string param) { }
    public virtual void InitValue()
    {
        if (runtime.lastStateDic[nodeId].Count == 0) return;

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
    
    public virtual void OnInitFinish() 
    {
        if (lastStates == null)
        {
            lastStates = new List<BehaviorTreeBaseState>();
            List<string> lastStateIds = runtime.lastStateDic[nodeId];
            foreach (string id in lastStateIds) lastStates.Add(runtime.stateDic[id]);
        }
    }
    public virtual void OnEnter() { InitValue(); state = EBTState.����; }

    public virtual void OnExecute() { state = EBTState.ִ����; }
    public virtual void OnExit() { state = EBTState.���; }
    public virtual void OnRefresh() 
    {
        state = EBTState.δ��ʼ;
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
    public T GetLastCoupleState<T>(Func<T, bool> selectFunc) where T : BehaviorTreeBaseState
    {
        T result = this as T;
        if (result != null && selectFunc(result)) return result;
        else
        {
            foreach (BehaviorTreeBaseState _state in lastStates)
            {
                if (_state.GetLastCoupleState(selectFunc) == null) continue;
                else return _state.GetLastCoupleState(selectFunc);
            }
        }
        return result;
    }
    /// <summary>
    /// ���Լ���ĳ�ֲ���������������ǰ�ýڵ㴫Ⱦ��ֱ����鵽�Ľڵ����ĳ������
    /// </summary>
    public void Infect(Action<BehaviorTreeBaseState> action,Func<BehaviorTreeBaseState,bool> checkFunc)
    {
        action(this);
        if (checkFunc(this)) return;
        foreach (BehaviorTreeBaseState _state in lastStates) 
        {
            _state.Infect(action,checkFunc);
        }
    }
    public virtual void OnUpdate()
    {
        if (state != EBTState.ִ����) goto wait;
        wait:;
    }

}
[Serializable]
public enum EBTState
{
    δ��ʼ,
    ����,
    ִ����,
    ���,
    �ȴ�,
}
[Serializable]
public struct SBTOutputInfo
{
    public string fromPortName;
    public string toPortName;
    public object value;
}