using System;
using System.Collections.Generic;
using UnityEngine;
using BTState = BehaviorTreeBaseState;

public class BTRuntimeComponent : MonoBehaviour
{
    public BTContainer container;
    public Dictionary<string, BTState> stateDic = new Dictionary<string, BTState>();
    public Dictionary<string, List<string>> lastStateDic = new Dictionary<string, List<string>>();
    private bool isInitFinish;

    private void Awake()
    {
        isInitFinish = false;
        LoadStates();
    }

    /// <summary>
    /// 加载行为树状态
    /// </summary>
    private void LoadStates()
    {
        foreach (NodeData nodeData in container.nodeDatas)
        {
            Type stateType = Type.GetType(nodeData.stateName);
            BTState btState = (BTState)Activator.CreateInstance(stateType);
            btState.InitParam(nodeData.stateParams);
            btState.stateName = nodeData.stateName;
            btState.runtime = this;
            btState.nodeId = nodeData.guid;
            stateDic.Add(nodeData.guid, btState);
            lastStateDic.Add(nodeData.guid, nodeData.lastNodes);
        }
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            state.OnInitFinish();
        }
        isInitFinish = true;
    }

    /// <summary>
    /// 设置行为树状态值
    /// </summary>
    /// <param name="action">要执行的动作</param>
    /// <param name="func">要应用的条件</param>
    public void SetStateValue(Action<BTState> action, Func<BTState, bool> func)
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            if (!func(state)) continue;
            action(state);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isInitFinish) return;
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            string nodeId = keyValuePair.Key;
            BTState checkState = keyValuePair.Value;
            if (lastStateDic[nodeId].Count == 0 && checkState.state == EBTState.未开始)
            {
                checkState.OnEnter();
            }
            else if (checkState.state == EBTState.未开始)
            {
                int checkCount = lastStateDic[nodeId].Count;
                foreach (string lastStateId in lastStateDic[nodeId])
                {
                    BTState lastState = stateDic[lastStateId];
                    if (lastState.state == EBTState.完成) checkCount--;
                    if (checkCount == 0) checkState.OnEnter();
                }
            }
            else if (checkState.state == EBTState.执行中)
            {
                checkState.OnUpdate();
            }
        }
    }
}
