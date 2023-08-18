using Codice.Client.Common.TreeGrouper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Node = UnityEditor.Experimental.GraphView.Node;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;

public abstract class BehaviorTreeBaseNode : Node
{
    public virtual string stateName => "DefaultState";
    public Vector2 nodePos;
    public string guid;
    public Type type;
    public GameObject target;
    public BehaviorTreeView graphView;
    public List<BehaviorTreeBaseNode> lastNodes = new List<BehaviorTreeBaseNode>();
    public Action<BehaviorTreeBaseNode> onSelectAction; //ѡ�нڵ�󣬽��ڵ���Ϣ���س�ȥ
    public BehaviorTreeBaseState btState;

    public BehaviorTreeBaseNode()
    {
        guid = Guid.NewGuid().ToSafeString();
        viewDataKey = guid;
    }
    public override void OnSelected()
    {
        base.OnSelected();
        onSelectAction?.Invoke(this);
    }
    public Port GetPortForNode(Node n, Direction portDir,Type type, Port.Capacity capacity = Port.Capacity.Single)
    {
        return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, type);
    }
    public Port GetPortByName(string name, Direction direction)
    {
        VisualElement checkContainer = direction == Direction.Output ? outputContainer : inputContainer;
        for (int i = 0; i < checkContainer.childCount; i++)
        {
            Port port = checkContainer[i].Q<Port>();
            if (port == null) continue;
            if (port.portName != name) continue;
            return port;
        }

        return null;
    }
}
public class BehaviorTreeBaseNode<IBTState>: BehaviorTreeBaseNode
{

}
