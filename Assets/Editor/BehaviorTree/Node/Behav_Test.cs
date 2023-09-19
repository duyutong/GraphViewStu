
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_Test : BehaviorNode
{
    public override string stateName => "TestState";
    public Behav_Test() : base() 
    {
        title = "Test";

        Port port_p0 = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_p0.portName = "p0";
        inputContainer.Add(port_p0);

        Port port_p2 = CreatePortForNode(this, Direction.Input, typeof(Single), Port.Capacity.Multi);
        port_p2.portName = "p2";
        inputContainer.Add(port_p2);


        Port port_p1 = CreatePortForNode(this, Direction.Output, typeof(Vector2), Port.Capacity.Multi);
        port_p1.portName = "p1";
        outputContainer.Add(port_p1);

        Port port_p3 = CreatePortForNode(this, Direction.Output, typeof(String), Port.Capacity.Single);
        port_p3.portName = "p3";
        outputContainer.Add(port_p3);

    }
}
