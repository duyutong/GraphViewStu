using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using Node = UnityEditor.Experimental.GraphView.Node;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using System;
using static UnityEditor.Experimental.GraphView.Port;

/// <summary>
/// ��Ϊ�������ڵ���
/// </summary>
[Serializable]
public abstract class BehaviorTreeBaseNode : Node
{
    /// <summary>
    /// ��ȡ�ڵ�ǰ׺
    /// </summary>
    public virtual string Prefix => "Base";

    /// <summary>
    /// ��ȡ״̬����
    /// </summary>
    public virtual string stateName => "DefaultState";

    public Vector2 nodePos;
    public string guid;
    public Type type;
    public GameObject target;
    public List<BehaviorTreeBaseNode> lastNodes = new List<BehaviorTreeBaseNode>();
    public Action<BehaviorTreeBaseNode> onSelectAction; //ѡ�нڵ�󣬽��ڵ���Ϣ���س�ȥ
    public Action onUnselected;//ȡ��ѡ��ص�
    public BehaviorTreeBaseState btState;

    /// <summary>
    /// ���캯��
    /// </summary>
    public BehaviorTreeBaseNode()
    {
        guid = Guid.NewGuid().ToSafeString();
        viewDataKey = guid;
    }

    /// <summary>
    /// ���ڵ㱻ѡ��ʱ����
    /// </summary>
    public override void OnSelected()
    {
        base.OnSelected();
        onSelectAction?.Invoke(this);
    }

    /// <summary>
    /// ���ڵ�ȡ��ѡ��ʱ����
    /// </summary>
    public override void OnUnselected()
    {
        base.OnUnselected();
        onUnselected?.Invoke();
    }

    /// <summary>
    /// Ϊ�ڵ㴴���˿�
    /// </summary>
    /// <param name="n">�ڵ�</param>
    /// <param name="portDir">�˿ڷ���</param>
    /// <param name="type">����</param>
    /// <param name="capacity">����</param>
    /// <returns>�˿�</returns>
    public Port CreatePortForNode(Node n, Direction portDir, Type type, Port.Capacity capacity = Port.Capacity.Single)
    {
        return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, type);
    }

    /// <summary>
    /// Ϊ�ڵ���Ӷ˿�
    /// </summary>
    /// <param name="setting">�˿�����</param>
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
    /// Ϊ�ڵ���Ӷ���˿�
    /// </summary>
    /// <param name="settings">�˿������б�</param>
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
    /// �ӽڵ����Ƴ�ָ������Ķ˿�
    /// </summary>
    /// <param name="name">�˿�����</param>
    /// <param name="direction">����</param>
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
    /// �������ƺͷ����ȡ�˿�
    /// </summary>
    /// <param name="name">�˿�����</param>
    /// <param name="direction">����</param>
    /// <returns>�˿�</returns>
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
/// ������Ϊ�������ڵ���
/// </summary>
public class BehaviorTreeBaseNode<IBTState> : BehaviorTreeBaseNode
{

}

/// <summary>
/// ��Ϊ���ڵ�˿�������
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
    /// ���ݶ˿����ͻ�ȡ��Ӧ��Type
    /// </summary>
    /// <returns>��Ӧ��Type</returns>
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
/// ��Ϊ���ڵ�˿�����ö��
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
