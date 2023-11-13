using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomNodeView : GraphView
{
    /// <summary>
    /// 当选择行为树节点时触发的事件
    /// </summary>
    public Action<BehaviorTreeBaseNode> onSelectAction;

    /// <summary>
    /// 当取消选择行为树节点时触发的事件
    /// </summary>
    public Action onUnselectAction;

    /// <summary>
    /// 剪贴板
    /// </summary>
    public string clipboard;

    /// <summary>
    /// 当前选中的节点
    /// </summary>
    private DefaultNode selectionNode = null;

    /// <summary>
    /// 获取是否可以粘贴内容
    /// </summary>
    protected override bool canPaste => !string.IsNullOrEmpty(clipboard);

    /// <summary>
    /// UXML工厂类，用于创建 CustomNodeView 实例
    /// </summary>
    public new class UxmlFactory : UxmlFactory<CustomNodeView, UxmlTraits> { }

    /// <summary>
    /// 构造函数，用于初始化 CustomNodeView 实例
    /// </summary>
    public CustomNodeView()
    {
        // 允许对Graph进行Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // 允许拖拽Content
        this.AddManipulator(new ContentDragger());
        // 允许拖拽Selection里的内容
        this.AddManipulator(new SelectionDragger());
        // GraphView允许进行框选
        this.AddManipulator(new RectangleSelector());

        clipboard = null;
    }

    /// <summary>
    /// 创建节点
    /// </summary>
    /// <param name="nodeName">节点名称</param>
    /// <param name="nodeType">节点类型</param>
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
    /// 构建菜单
    /// </summary>
    /// <param name="evt">菜单事件</param>
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
    /// 复制选中的节点
    /// </summary>
    /// <param name="node">要复制的节点</param>
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
    /// 剪切选中的节点
    /// </summary>
    /// <param name="action">下拉菜单动作</param>
    private void CutSelectionCallback(DropdownMenuAction action)
    {
        CopySelectionCallback(selectionNode);
        RemoveElement(selectionNode);
    }

    /// <summary>
    /// 粘贴节点
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
