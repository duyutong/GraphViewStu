using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LoopDeco : DecoratorNode
{
    public override string stateName => "LoopState";
    public LoopDeco() : base() 
    {
        title = "Loop";

        Port ePort = GetPortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Multi);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port oPort = GetPortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
