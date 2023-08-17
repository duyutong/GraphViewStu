using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class StartTrigger : TriggerNode
{
    public override string stateName => "StartState";
    public StartTrigger() : base()
    {
        title = "Start";
        Port oPort = GetPortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "next";
        outputContainer.Add(oPort);
    }
}
