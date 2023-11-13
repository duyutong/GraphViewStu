
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_InterruptTest : BehaviorNode
{
    public override string stateName => "InterruptTestState";
    public Behav_InterruptTest() : base() 
    {
        title = "InterruptTest";

        Port port_interruptTag = CreatePortForNode(this, Direction.Input, typeof(String), Port.Capacity.Multi);
        port_interruptTag.portName = "interruptTag";
        inputContainer.Add(port_interruptTag);


    }
}
