using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class InputTrigger : TriggerNode
{
    public override string stateName => "InputState";
    public InputTrigger() : base() 
    {
        title = "Input";
        Port oPort = GetPortForNode(this, Direction.Output, typeof(Vector2), Port.Capacity.Single);
        oPort.portName = "result";
        outputContainer.Add(oPort);

        Port ePort = GetPortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Single);
        ePort.portName = "enter";
        inputContainer.Add(ePort);
    }
}
