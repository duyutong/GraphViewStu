using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
using UnityEngine;

public class Behav_Move : BehaviorNode
{
    public override string stateName => "MoveState";
    public Behav_Move() : base()
    {
        title = "Move";
       
        Port ePort = GetPortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Single);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port speedPort = GetPortForNode(this, Direction.Input, typeof(float), Port.Capacity.Single);
        speedPort.portName = "speed";
        inputContainer.Add(speedPort);

        Port dirPort = GetPortForNode(this, Direction.Input, typeof(Vector2), Port.Capacity.Single);
        dirPort.portName = "direction";
        inputContainer.Add(dirPort);


        Port tPort = GetPortForNode(this, Direction.Output, typeof(Vector3), Port.Capacity.Multi);
        tPort.portName = "targetPos";
        outputContainer.Add(tPort);

        Port oPort = GetPortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
