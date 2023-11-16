
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Deco_TimeDelay : DecoratorNode
{
    public override string stateName => "TimeDelayState";
    public Deco_TimeDelay() : base() 
    {
        title = "TimeDelay";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_delayTime = CreatePortForNode(this, Direction.Input, typeof(Single), Port.Capacity.Single);
        port_delayTime.portName = "delayTime";
        inputContainer.Add(port_delayTime);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
