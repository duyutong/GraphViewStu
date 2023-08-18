using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorNode : BehaviorTreeBaseNode<BehaviorTreeBaseState>
{
    public BehaviorNode() : base() 
    {
        title = "*BehaviorNode";
    }
}
