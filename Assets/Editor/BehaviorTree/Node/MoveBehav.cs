using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using UnityEditor.Experimental.GraphView;
using UnityEditor.TerrainTools;
using UnityEngine;

public class MoveBehav : BehaviorNode
{
    public override string stateName => "MoveState";
    public MoveBehav() : base()
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


        Port oPort = GetPortForNode(this, Direction.Output, typeof(Vector3), Port.Capacity.Single);
        oPort.portName = "targetPos";
        outputContainer.Add(oPort);
    }
}
