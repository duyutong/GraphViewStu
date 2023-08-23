using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class CustomNodeView : GraphView
{
    public Action<BehaviorTreeBaseNode> onSelectAction;
    public Action onUnselectAction;

    public new class UxmlFactory : UxmlFactory<CustomNodeView, UxmlTraits> { }
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
    }
    public void CreatNode(string nodeName,string nodeType)
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
}
