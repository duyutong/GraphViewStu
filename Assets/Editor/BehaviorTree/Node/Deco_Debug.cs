using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Deco_Debug : DecoratorNode
{
    public override string stateName => "DebugState";
    public Deco_Debug() : base() 
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
