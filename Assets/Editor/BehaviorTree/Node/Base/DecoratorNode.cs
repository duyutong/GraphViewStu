using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorNode : BehaviorTreeBaseNode<BehaviorTreeBaseState>
{
    public override string Prefix => "Deco";
    public DecoratorNode() : base()
    {
        title = "*DecoratorNode";
    }
}
