
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Deco_LogicGate : DecoratorNode
{
    public override string stateName => "LogicGateState";
    public Deco_LogicGate() : base() 
    {
        title = "LogicGate";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
