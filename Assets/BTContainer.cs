using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class BTContainer : ScriptableObject
{
    [HideInInspector]
    public List<NodeData> nodeDatas = new List<NodeData>();
    [HideInInspector]
    public List<EdgeData>edgeDatas = new List<EdgeData>();
}
[Serializable]
public class BTContainer_Copy
{
    [HideInInspector]
    public List<NodeData> nodeDatas = new List<NodeData>();
    [HideInInspector]
    public List<EdgeData> edgeDatas = new List<EdgeData>();
}
[Serializable]
public class NodeData 
{
    public string nodeName;
    public string guid;
    public List<string> lastNodes;
    public List<SBTOutputInfo> output;
    public Vector2 nodePos;
    public string typeName;
    public string stateName;
    public string stateParams;//json
}
[Serializable]
public class EdgeData
{
    public string outPortNode;
    public string intputPortNode;

    public string outPortName;
    public string intputPortName;
}



