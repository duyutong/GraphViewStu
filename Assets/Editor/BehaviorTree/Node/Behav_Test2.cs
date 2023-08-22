
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_Test2 : BehaviorNode
{
    public override string stateName => "Test2State";
    public Behav_Test2() : base() 
    {
        title = "Test2";

        Port port_p1 = GetPortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_p1.portName = "p1";
        inputContainer.Add(port_p1);

        Port port_p2 = GetPortForNode(this, Direction.Input, typeof(Vector2), Port.Capacity.Single);
        port_p2.portName = "p2";
        inputContainer.Add(port_p2);


        Port port_p3 = GetPortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_p3.portName = "p3";
        outputContainer.Add(port_p3);

    }
}
