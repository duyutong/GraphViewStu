using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using BTState = BehaviorTreeBaseState;
public class BTRuntimeComponent : MonoBehaviour
{
    public BTContainer container;
    public Dictionary<string, BTState> stateDic = new Dictionary<string, BTState>();
    private void Awake()
    {
        LoadStates();
    }

    private void LoadStates()
    {
        Dictionary<string, List<string>> tempDic = new Dictionary<string, List<string>>();
        foreach (NodeData nodeData in container.nodeDatas) 
        {
            Type stateType = Type.GetType(nodeData.stateName);
            BTState btState = (BTState)Activator.CreateInstance(stateType);
            btState.Init(nodeData.stateParams);
            btState.runtime = this;
            stateDic.Add(nodeData.guid, btState);

            foreach (string lastNodeId in nodeData.lastNodes) 
            {
                if (tempDic.ContainsKey(lastNodeId)) tempDic[lastNodeId].Add(nodeData.guid);
                else tempDic.Add(lastNodeId, new List<string>() { nodeData.guid });
            }
        }
        foreach (KeyValuePair<string, List<string>> keyValuePair in tempDic) 
        {
            string lastNodeId = keyValuePair.Key;
            BTState lastState = stateDic[lastNodeId];
            foreach (string nodeId in keyValuePair.Value) 
                stateDic[nodeId].lastStates.Add(lastState);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        BTState statrState = stateDic[container.startGuid];
        statrState.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic) 
        {
            BTState checkState = keyValuePair.Value;
            if (checkState.state != EBTState.执行中) 
            {
                int checkCount = checkState.lastStates.Count;
                foreach (BTState lastState in checkState.lastStates)
                {
                    if (lastState.state == EBTState.完成) checkCount--;
                    if (checkCount == 0) checkState.OnEnter();
                }   
            }
            else if (checkState.state == EBTState.执行中) checkState.OnUpdate();
        }
    }
}
