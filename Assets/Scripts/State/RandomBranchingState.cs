
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;

[Serializable]
public class RandomBranchingState : BehaviorTreeBaseState
{
    public int parallelCount;
    public List<BehaviorTreeBaseState> nextStates;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<RandomBranchingStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.parallelCount = parallelCount;
            }
            return _stateObj;
        }
    }
    private RandomBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(RandomBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (RandomBranchingStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            parallelCount = _stateObj.parallelCount;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        parallelCount = _stateObj.parallelCount;
    }
    public override void OnInitFinish()
    {
        nextStates = new List<BehaviorTreeBaseState>();
        List<string> stateIds = new List<string>();
        foreach (KeyValuePair<string, List<string>> keyValuePair in runtime.lastStateDic) 
        {
            string stateId = keyValuePair.Key;
            if (stateId == nodeId) continue;
            List<string> values = keyValuePair.Value;
            if (!values.Contains(nodeId)) continue;
            if (stateIds.Contains(stateId)) continue;
            stateIds.Add(stateId);
        }

        foreach (string _id in stateIds) 
        {
            nextStates.Add(runtime.stateDic[_id]);
        }
    }
    public override void OnEnter()
    {
        base.OnEnter();

        if (nextStates.Count > 0) 
        {
            List<int> randomNumbers = GenerateRandomNumbers(0, nextStates.Count, parallelCount);
            for (int i = 0; i < nextStates.Count; i++) 
            {
                int _index = i;
                bool isSelect = randomNumbers.Contains(_index);

                if (!isSelect) nextStates[_index].state = EBTState.Пе;
                else nextStates[_index].OnRefresh();
            }
        }
        
        base.OnExit();

    }
    private List<int> GenerateRandomNumbers(int minValue, int maxValue, int n)
    {
        if (n > maxValue - minValue + 1 || n < 0)
        {
            throw new ArgumentException("Invalid input parameters. Unable to generate unique random numbers.");
        }

        List<int> randomNumbers = new List<int>();
        System.Random random = new System.Random();

        while (randomNumbers.Count < n)
        {
            int randomNumber = random.Next(minValue, maxValue);
            if (!randomNumbers.Contains(randomNumber))
            {
                randomNumbers.Add(randomNumber);
            }
        }
        
        return randomNumbers;
    }
}
public class RandomBranchingStateObj : BTStateObject
{
    public EBTState state;
    public int parallelCount;

    public List<BTOutputInfo> output;
}
