using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Node = UnityEditor.Experimental.GraphView.Node;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using static UnityEditor.Experimental.GraphView.Port;

/// <summary>
/// 行为树基础节点类
/// </summary>
[Serializable]
public abstract class BehaviorTreeBaseNode : Node
{
    /// <summary>
    /// 获取节点前缀
    /// </summary>
    public virtual string Prefix => "Base";

    /// <summary>
    /// 获取状态名称
    /// </summary>
    public virtual string stateName => "DefaultState";

    public Vector2 nodePos;
    public string guid;
    public Type type;
    public GameObject target;
    public List<BehaviorTreeBaseNode> lastNodes = new List<BehaviorTreeBaseNode>();
    public Action<BehaviorTreeBaseNode> onSelectAction; //选中节点后，将节点信息返回出去
    public Action onUnselected;//取消选择回调
    public BehaviorTreeBaseState btState;

    /// <summary>
    /// 构造函数
    /// </summary>
    public BehaviorTreeBaseNode()
    {
        guid = Guid.NewGuid().ToSafeString();
        viewDataKey = guid;
    }

    /// <summary>
    /// 当节点被选中时调用
    /// </summary>
    public override void OnSelected()
    {
        base.OnSelected();
        onSelectAction?.Invoke(this);
    }

    /// <summary>
    /// 当节点取消选择时调用
    /// </summary>
    public override void OnUnselected()
    {
        base.OnUnselected();
        onUnselected?.Invoke();
    }

    /// <summary>
    /// 为节点创建端口
    /// </summary>
    /// <param name="n">节点</param>
    /// <param name="portDir">端口方向</param>
    /// <param name="type">类型</param>
    /// <param name="capacity">容量</param>
    /// <returns>端口</returns>
    public Port CreatePortForNode(Node n, Direction portDir, Type type, Port.Capacity capacity = Port.Capacity.Single)
    {
        return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, type);
    }

    /// <summary>
    /// 为节点添加端口
    /// </summary>
    /// <param name="setting">端口设置</param>
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

    /// <summary>
    /// 为节点添加多个端口
    /// </summary>
    /// <param name="settings">端口设置列表</param>
    public void AddPortForNode(List<BTNodePortSetting> settings)
    {
        BehaviorTreeBaseNode node = this;
        foreach (BTNodePortSetting setting in settings)
        {
            Type portType = setting.GetTypeByEPortType();
            Port port = node.InstantiatePort(Orientation.Horizontal, setting.direction, setting.capacity, portType);
            port.portName = setting.portName;

            if (setting.direction == Direction.Output) node.outputContainer.Add(port);
            if (setting.direction == Direction.Input) node.inputContainer.Add(port);
        }

        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    /// <summary>
    /// 从节点中移除指定方向的端口
    /// </summary>
    /// <param name="name">端口名称</param>
    /// <param name="direction">方向</param>
    public void RemovePortFromNode(string name, Direction direction)
    {
        BehaviorTreeBaseNode node = this;
        VisualElement checkContainer = direction == Direction.Output ? outputContainer : inputContainer;
        Port port = GetPortByName(name, direction);
        checkContainer.Remove(port);

        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    /// <summary>
    /// 根据名称和方向获取端口
    /// </summary>
    /// <param name="name">端口名称</param>
    /// <param name="direction">方向</param>
    /// <returns>端口</returns>
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

/// <summary>
/// 泛型行为树基础节点类
/// </summary>
public class BehaviorTreeBaseNode<IBTState> : BehaviorTreeBaseNode
{

}

/// <summary>
/// 行为树节点端口设置类
/// </summary>
[Serializable]
public class BTNodePortSetting
{
    public BehaviorTreeBaseNode node;
    public string portName;
    public Direction direction;
    public Capacity capacity;
    public EPortType portType;

    /// <summary>
    /// 根据端口类型获取对应的Type
    /// </summary>
    /// <returns>对应的Type</returns>
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

/// <summary>
/// 行为树节点端口类型枚举
/// </summary>
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
