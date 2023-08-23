using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DefaultNode : BehaviorTreeBaseNode
{
    public string nodeType;
    public override string stateName => "Null";
    public DefaultNode() : base()
    {
        title = "*TempNode";
    }
}
