
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Deco_RandomBranching : DecoratorNode
{
    public override string stateName => "RandomBranchingState";
    public Deco_RandomBranching() : base() 
    {
        title = "RandomBranching";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_parallelCount = CreatePortForNode(this, Direction.Input, typeof(int), Port.Capacity.Single);
        port_parallelCount.portName = "parallelCount";
        inputContainer.Add(port_parallelCount);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
