using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomNodeView : GraphView
{
    /// <summary>
    /// ��ѡ����Ϊ���ڵ�ʱ�������¼�
    /// </summary>
    public Action<BehaviorTreeBaseNode> onSelectAction;

    /// <summary>
    /// ��ȡ��ѡ����Ϊ���ڵ�ʱ�������¼�
    /// </summary>
    public Action onUnselectAction;

    /// <summary>
    /// ������
    /// </summary>
    public string clipboard;

    /// <summary>
    /// ��ǰѡ�еĽڵ�
    /// </summary>
    private DefaultNode selectionNode = null;

    /// <summary>
    /// ��ȡ�Ƿ����ճ������
    /// </summary>
    protected override bool canPaste => !string.IsNullOrEmpty(clipboard);

    /// <summary>
    /// UXML�����࣬���ڴ��� CustomNodeView ʵ��
    /// </summary>
    public new class UxmlFactory : UxmlFactory<CustomNodeView, UxmlTraits> { }

    /// <summary>
    /// ���캯�������ڳ�ʼ�� CustomNodeView ʵ��
    /// </summary>
    public CustomNodeView()
    {
        // �����Graph����Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // ������קContent
        this.AddManipulator(new ContentDragger());
        // ������קSelection�������
        this.AddManipulator(new SelectionDragger());
        // GraphView������п�ѡ
        this.AddManipulator(new RectangleSelector());

        clipboard = null;
    }

    /// <summary>
    /// �����ڵ�
    /// </summary>
    /// <param name="nodeName">�ڵ�����</param>
    /// <param name="nodeType">�ڵ�����</param>
    public void CreatNode(string nodeName, string nodeType)
    {
        DefaultNode node = new DefaultNode();
        node.title = nodeName;
        node.nodeType = nodeType;
        node.onSelectAction = onSelectAction;
        node.onUnselected = onUnselectAction;
        node.nodePos = default;
        node.SetPosition(new Rect(node.nodePos, node.GetPosition().size));

        AddElement(node);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    /// <summary>
    /// ���������Ĳ˵�
    /// </summary>
    /// <param name="evt">�����Ĳ˵��¼�</param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is DefaultNode)
        {
            selectionNode = (DefaultNode)evt.target;
            evt.menu.AppendAction("Copy", delegate
            {
                CopySelectionCallback(selectionNode);
            }, DropdownMenuAction.AlwaysEnabled);
        }
        if (evt.target is DefaultNode)
        {
            selectionNode = (DefaultNode)evt.target;
            evt.menu.AppendAction("Cut", CutSelectionCallback, DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }
        if (evt.target is GraphView)
        {
            evt.menu.AppendAction("Paste", delegate
            {
                OnPasteCallback();
            }, (DropdownMenuAction a) => canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }
    }

    /// <summary>
    /// ����ѡ�еĽڵ�
    /// </summary>
    /// <param name="node">Ҫ���ƵĽڵ�</param>
    private void CopySelectionCallback(DefaultNode node)
    {
        CustomNodeData customNodeData = new CustomNodeData();

        NodeData nodeData = new NodeData();
        nodeData.nodeName = node.title;
        nodeData.typeName = node.nodeType;
        nodeData.nodePos = node.GetPosition().position;

        List<BTNodePortSetting> portSettings = new List<BTNodePortSetting>();
        List<Port> ports = node.inputContainer.Query<Port>().ToList();
        ports.AddRange(node.outputContainer.Query<Port>().ToList());
        foreach (Port port in ports)
        {
            BTNodePortSetting info = new BTNodePortSetting();
            info.portName = port.portName;
            info.direction = port.direction;
            info.capacity = port.capacity;
            info.portType = (EPortType)Enum.Parse(typeof(EPortType), port.portType.Name);
            portSettings.Add(info);
        }
        customNodeData.nodeData = nodeData;
        customNodeData.portSettings = portSettings;

        clipboard = GraphSaveUtility.SerializeObject(customNodeData);
    }

    /// <summary>
    /// ����ѡ�еĽڵ�
    /// </summary>
    /// <param name="action">�����˵�����</param>
    private void CutSelectionCallback(DropdownMenuAction action)
    {
        CopySelectionCallback(selectionNode);
        RemoveElement(selectionNode);
    }

    /// <summary>
    /// ճ���ڵ�
    /// </summary>
    private void OnPasteCallback()
    {
        if (canPaste == false) return;
        CustomNodeData customNodeData = GraphSaveUtility.DeserializeObject<CustomNodeData>(clipboard);
        NodeData nodeData = customNodeData.nodeData;

        DefaultNode node = new DefaultNode();
        node.title = nodeData.nodeName;
        node.nodeType = nodeData.typeName;
        node.nodePos = nodeData.nodePos + 10 * Vector2.one;
        node.SetPosition(new Rect(node.nodePos, node.GetPosition().size));
        node.onSelectAction = onSelectAction;
        node.onUnselected = onUnselectAction;
        node.AddPortForNode(customNodeData.portSettings);

        AddElement(node);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }
}
