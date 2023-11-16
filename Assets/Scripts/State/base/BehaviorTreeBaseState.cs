using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class BehaviorTreeBaseState
{
    public EBTState state = EBTState.未开始;
    public string stateName = "*BehaviorTreeBaseState";
    public string nodeId;
    public bool isBranch;
    public bool interruptible = false;
    public string interruptTag = "";
    public BTRuntimeComponent runtime;
    public List<BTOutputInfo> output = new List<BTOutputInfo>();
    public List<BehaviorTreeBaseState> lastStates;
    public virtual BTStateObject stateObj { get; }

    public Action onEnterForRuntime;
    public Action onExitForRuntime;

    /// <summary>
    /// 初始化参数
    /// </summary>
    /// <param name="param">参数</param>
    public virtual void InitParam(string param) {}

    /// <summary>
    /// 初始化值
    /// </summary>
    public virtual void InitValue()
    {
        if (runtime.lastStateDic[nodeId].Count == 0) return;

        Type type = GetType();
        foreach (BehaviorTreeBaseState lastState in lastStates)
        {
            foreach (BTOutputInfo inputInfo in lastState.output)
            {
                if (inputInfo.value == null) continue;
                string memberName = inputInfo.toPortName;
                FieldInfo fieldInfo = type.GetField(memberName);
                if (fieldInfo == null) continue;
                fieldInfo.SetValue(this, inputInfo.value);
            }
        }
    }

    /// <summary>
    /// 刷新输出
    /// </summary>
    /// <param name="newInfo">新信息</param>
    /// <param name="isRemove">是否移除</param>
    public void RefreshOutput(BTOutputInfo newInfo, bool isRemove)
    {
        if (isRemove)
        {
            for (int i = output.Count - 1; i > 0; i--)
            {
                BTOutputInfo info = output[i];
                if (info.fromPortName != newInfo.fromPortName) continue;
                if (info.toPortName != newInfo.toPortName) continue;
                output.Remove(info);
            }
        }
        else
            output.Add(newInfo);
    }

    /// <summary>
    /// 保存状态
    /// </summary>
    public virtual void Save() { }

    public virtual void OnInitLastStates() 
    {
        if (lastStates == null)
        {
            lastStates = new List<BehaviorTreeBaseState>();
            List<string> lastStateIds = runtime.lastStateDic[nodeId];
            foreach (string id in lastStateIds) lastStates.Add(runtime.stateDic[id]);
        }
    }
    public virtual void OnInitFinish(){ }
    /// <summary>
    /// 进入状态回调
    /// </summary>
    public virtual void OnEnter() 
    {
        InitValue();
        state = EBTState.进入; 
        onEnterForRuntime?.Invoke();
    }

    /// <summary>
    /// 执行状态回调
    /// </summary>
    public virtual void OnExecute() { state = EBTState.执行中; }

    /// <summary>
    /// 退出状态回调
    /// </summary>
    public virtual void OnExit() { state = EBTState.完成; onExitForRuntime?.Invoke(); }

    /// <summary>
    /// 打断
    /// </summary>
    public virtual void OnInterrupt() { state = EBTState.中断; }
    /// <summary>
    /// 回复打断状态
    /// </summary>
    public virtual void OnResume() { state = EBTState.执行中; }
    /// <summary>
    /// 刷新状态回调
    /// </summary>
    public virtual void OnRefresh()
    {
        state = EBTState.未开始;
        ClearOutputList();
    }

    /// <summary>
    /// 清除输出列表
    /// </summary>
    public virtual void ClearOutputList()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            info.value = null;
            output[i] = info;
        }
    }

    /// <summary>
    /// 获取上一个符合条件的状态
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="selectFunc">选择条件函数</param>
    /// <returns>符合条件的状态</returns>
    public T GetLastMatchingState<T>(Func<T, bool> selectFunc) where T : BehaviorTreeBaseState
    {
        T result = this as T;
        if (result != null && selectFunc(result)) return result;
        else
        {
            foreach (BehaviorTreeBaseState _state in lastStates)
            {
                if (_state.GetLastMatchingState(selectFunc) == null) continue;
                else return _state.GetLastMatchingState(selectFunc);
            }
        }
        return result;
    }
    /// <summary>
    /// 将操作传递到前置节点，直到检查到的节点符合条件
    /// </summary>
    /// <param name="action">操作函数</param>
    /// <param name="checkFunc">检查条件函数</param>
    public void Infect(Action<BehaviorTreeBaseState> action, Func<BehaviorTreeBaseState, bool> checkFunc)
    {
        action(this);
        if (checkFunc(this)) return;
        foreach (BehaviorTreeBaseState _state in lastStates)
        {
            _state.Infect(action, checkFunc);
        }
    }

    /// <summary>
    /// 更新状态回调
    /// </summary>
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
    中断,
    空,       //当作为随机分支时，状态为空时，代表被放弃的分支
}

[Serializable]
public class BTOutputInfo
{
    public string fromPortName;
    public string toPortName;
    public object value;
}
