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

        Port ePort = CreatePortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Multi);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port oPort = CreatePortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Single);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
