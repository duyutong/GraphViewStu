using Codice.Client.Common.TreeGrouper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Node = UnityEditor.Experimental.GraphView.Node;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using static UnityEditor.Experimental.GraphView.Port;

public abstract class BehaviorTreeBaseNode : Node
{
    public virtual string stateName => "DefaultState";
    public Vector2 nodePos;
    public string guid;
    public Type type;
    public GameObject target;
    public List<BehaviorTreeBaseNode> lastNodes = new List<BehaviorTreeBaseNode>();
    public Action<BehaviorTreeBaseNode> onSelectAction; //选中节点后，将节点信息返回出去
    public Action onUnselected;//取消选择回调
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
    public override void OnUnselected()
    {
        base.OnUnselected();
        onUnselected?.Invoke();
    }
    public Port GetPortForNode(Node n, Direction portDir, Type type, Port.Capacity capacity = Port.Capacity.Single)
    {
        return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, type);
    }
    public void AddPortForNode(BTNodePortSetting setting)
    {
        BehaviorTreeBaseNode node = this;
        Type portType = setting.GetTypeByEPortType();
        Port port = node.InstantiatePort(Orientation.Horizontal, setting.direction, setting.capacity, portType);
        port.portName = setting.portName;

        if (setting.direction == Direction.Output) node.outputContainer.Add(port);
        if (setting.direction == Direction.Input) node.inputContainer.Add(port);

        node.RefreshExpandedState();
        node.RefreshPorts();
    }
    public void RemovePortFromNode(string name, Direction direction)
    {
        BehaviorTreeBaseNode node = this;
        VisualElement checkContainer = direction == Direction.Output ? outputContainer : inputContainer;
        Port port = GetPortByName(name, direction);
        checkContainer.Remove(port);

        node.RefreshExpandedState();
        node.RefreshPorts();
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
public class BehaviorTreeBaseNode<IBTState> : BehaviorTreeBaseNode
{

}
[Serializable]
public class BTNodePortSetting
{
    public BehaviorTreeBaseNode node;
    public string portName;
    public Direction direction;
    public Capacity capacity;
    public EPortType portType;

    public Type GetTypeByEPortType()
    {
        switch (portType)
        {
            case EPortType.Boolean: return typeof(bool);
            case EPortType.Float: return typeof(float);
            case EPortType.Vector3: return typeof(Vector3);
            case EPortType.Vector4: return typeof(Vector4);
            case EPortType.Vector2: return typeof(Vector2);
            case EPortType.String: return typeof(string);
        }
        return typeof(string);
    }
}
[Serializable]
public enum EPortType
{
    Boolean,
    Vector2,
    Vector3,
    Vector4,
    Float,
    String,
}
[Serializable]
public enum ENodeType
{
    Behavior,
    Decorator,
    Trigger,
}

