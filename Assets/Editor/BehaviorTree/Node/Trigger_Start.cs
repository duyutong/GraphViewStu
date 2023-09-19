using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Trigger_Start : TriggerNode
{
    public override string stateName => "StartState";
    public Trigger_Start() : base()
    {
        title = "Start";
        Port oPort = CreatePortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "next";
        outputContainer.Add(oPort);
    }
}
