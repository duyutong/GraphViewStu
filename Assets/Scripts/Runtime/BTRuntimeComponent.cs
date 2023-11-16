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
    private int componentIndex;
    private void Awake()
    {
        isInitFinish = false;
        LoadStates();
        BTRuntimeController.AddRuntime(this, (_index) => { componentIndex = _index; });
    }
    /// <summary>
    /// ������Ϊ��״̬
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
            state.OnInitLastStates();
        }
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            state.OnInitFinish();
        }
        isInitFinish = true;
    }

    /// <summary>
    /// ������Ϊ��״ֵ̬
    /// </summary>
    /// <param name="action">Ҫִ�еĶ���</param>
    /// <param name="func">ҪӦ�õ�����</param>
    public void SetStateValue(Action<BTState> action, Func<BTState, bool> func)
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            if (!func(state)) continue;
            action(state);
        }
    }
    public void OnReceiveMsg(string stateTag, EBTState eBTState = EBTState.�ж�)
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState checkState = keyValuePair.Value;
            //Ŀǰֻ������
            if (eBTState == EBTState.�ж�)
            {
                if (checkState.state != EBTState.ִ����) continue;
                if (!checkState.interruptible) continue;
                if (checkState.interruptTag != stateTag) continue;
                checkState.OnInterrupt();
            }
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
            bool isLogicGate = (checkState as LogicGateState) != null;
            if (lastStateDic[nodeId].Count == 0 && checkState.state == EBTState.δ��ʼ)
            {
                checkState.OnEnter();
            }
            else if (checkState.state == EBTState.δ��ʼ)
            {
                int checkCount = lastStateDic[nodeId].Count;
                bool isExistFinish = false;
                foreach (string lastStateId in lastStateDic[nodeId])
                {
                    BTState lastState = stateDic[lastStateId];
                    if (lastState.state == EBTState.���) { checkCount--; isExistFinish = true; }
                    if ((isLogicGate && isExistFinish) || checkCount == 0) checkState.OnEnter();
                }
            }
            else if (checkState.state == EBTState.ִ����)
            {
                checkState.OnUpdate();
            }
        }
    }
    private void OnDestroy()
    {
        BTRuntimeController.RemoveRuntime(componentIndex);
    }
}
