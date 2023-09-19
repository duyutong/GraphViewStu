using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Deco_Loop : DecoratorNode
{
    public override string stateName => "LoopState";
    public Deco_Loop() : base() 
    {
        title = "Loop";

        Port ePort = CreatePortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Multi);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port oPort = CreatePortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
