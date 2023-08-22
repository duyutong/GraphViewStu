using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TempNode : BehaviorTreeBaseNode
{
    public override string stateName => "Null";
    public ENodeType nodeType;
    public TempNode() : base()
    {
        title = "*TempNode";
    }
}
