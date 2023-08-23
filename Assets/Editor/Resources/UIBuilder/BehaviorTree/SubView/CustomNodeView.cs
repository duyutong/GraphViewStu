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
        // �����Graph����Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // ������קContent
        this.AddManipulator(new ContentDragger());
        // ������קSelection�������
        this.AddManipulator(new SelectionDragger());
        // GraphView������п�ѡ
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
