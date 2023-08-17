using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DebugDeco : DecoratorNode
{
    public override string stateName => "DebugState";
    public DebugDeco() : base() 
    {
        title = "Debug";

        Port ePort = GetPortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Single);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port oPort = GetPortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Single);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
