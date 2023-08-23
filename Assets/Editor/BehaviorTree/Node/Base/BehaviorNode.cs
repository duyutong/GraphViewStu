using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorNode : BehaviorTreeBaseNode<BehaviorTreeBaseState>
{
    public override string Prefix => "Behav";
    public BehaviorNode() : base() 
    {
        title = "*BehaviorNode";
    }
}
