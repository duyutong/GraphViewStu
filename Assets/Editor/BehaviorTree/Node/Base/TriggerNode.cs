using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TriggerNode : BehaviorTreeBaseNode<BehaviorTreeBaseState>
{
    public override string Prefix => "Trigger";
    public TriggerNode() : base()
    {
        title = "*TiggerNode";
    }
}
