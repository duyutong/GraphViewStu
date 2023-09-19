using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Trigger_Input : TriggerNode
{
    public override string stateName => "InputState";
    public Trigger_Input() : base() 
    {
        title = "Input";
        Port oPort = CreatePortForNode(this, Direction.Output, typeof(Vector2), Port.Capacity.Single);
        oPort.portName = "result";
        outputContainer.Add(oPort);

        Port ePort = CreatePortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Single);
        ePort.portName = "enter";
        inputContainer.Add(ePort);
    }
}
