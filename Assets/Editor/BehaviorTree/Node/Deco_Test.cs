
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Deco_Test : DecoratorNode
{
    public override string stateName => "TestState";
    public Deco_Test() : base() 
    {
        title = "Test";

        Port port_enter = GetPortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_dir = GetPortForNode(this, Direction.Input, typeof(Vector3), Port.Capacity.Single);
        port_dir.portName = "dir";
        inputContainer.Add(port_dir);

        Port port_speed = GetPortForNode(this, Direction.Input, typeof(Single), Port.Capacity.Single);
        port_speed.portName = "speed";
        inputContainer.Add(port_speed);


        Port port_debug = GetPortForNode(this, Direction.Output, typeof(String), Port.Capacity.Multi);
        port_debug.portName = "debug";
        outputContainer.Add(port_debug);

        Port port_exit = GetPortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
