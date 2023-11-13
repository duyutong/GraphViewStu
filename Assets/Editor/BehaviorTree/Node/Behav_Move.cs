using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Behav_Move : BehaviorNode
{
    public override string stateName => "MoveState";
    public Behav_Move() : base()
    {
        title = "Move";
       
        Port ePort = CreatePortForNode(this, Direction.Input, typeof(bool), Port.Capacity.Single);
        ePort.portName = "enter";
        inputContainer.Add(ePort);

        Port speedPort = CreatePortForNode(this, Direction.Input, typeof(float), Port.Capacity.Single);
        speedPort.portName = "speed";
        inputContainer.Add(speedPort);

        Port dirPort = CreatePortForNode(this, Direction.Input, typeof(Vector2), Port.Capacity.Single);
        dirPort.portName = "direction";
        inputContainer.Add(dirPort);

        Port tPort = CreatePortForNode(this, Direction.Output, typeof(Vector3), Port.Capacity.Multi);
        tPort.portName = "targetPos";
        outputContainer.Add(tPort);

        Port oPort = CreatePortForNode(this, Direction.Output, typeof(bool), Port.Capacity.Multi);
        oPort.portName = "exit";
        outputContainer.Add(oPort);
    }
}
