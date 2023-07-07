using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// ����dialogue graph�ĵײ���
public class DialogueGraphView : GraphView
{
    // �ڹ��캯�����GraphView����һЩ��ʼ������
    public DialogueGraphView()
    {
        // �����Graph����Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // ������קContent
        this.AddManipulator(new ContentDragger());
        // ������קSelection�������
        this.AddManipulator(new SelectionDragger());
        // GraphView������п�ѡ
        this.AddManipulator(new RectangleSelector());

        // 1. ����StartNode�������ú���position
        var startNode = GenEntryPointNode();
        // 2. ��node���뵽GraphView��
        AddElement(startNode);
        // 3. ��StartNode���Output Port
        var port = GenPortForNode(startNode, Direction.Output, Port.Capacity.Single);
        // 4. ��output����
        port.portName = "Next";
        // 5. ���뵽StartNode��outputContainer��
        startNode.outputContainer.Add(port);
        // ��������refresh����
        startNode.RefreshExpandedState();
        startNode.RefreshPorts();
    }
    // �Ƚϼ򵥣��൱��new��һ��Node
    private DialogueNode GenEntryPointNode()
    {
        DialogueNode node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),// ����System��Guid���ɷ���
            Text = "ENTRYPOINT",
            Entry = true
        };
        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));
        //node.onSelectAction = (_node) => { Debug.Log(_node.title); };
        return node;
    }

    // Ϊ�ڵ�n����input port����output port
    // Direction: ��һ���򵥵�ö�٣���ΪInput��Output����
    private Port GenPortForNode(Node n, Direction portDir, Port.Capacity capacity = Port.Capacity.Single)
    {
        // OrientationҲ�Ǹ��򵥵�ö�٣���ΪHorizontal��Vertical���֣�port������������float
        return n.InstantiatePort(Orientation.Horizontal, portDir, capacity, typeof(float));
    }
    public void AddDialogueNode(string nodeName)
    {
        // 1. ����Node
        DialogueNode node = new DialogueNode
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString(),
            Text = nodeName,
            Entry = false
        };
        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));

        // 2. Ϊ�䴴��InputPort
        var iport = GenPortForNode(node, Direction.Input, Port.Capacity.Multi);
        iport.portName = "input";
        node.inputContainer.Add(iport);
        node.RefreshExpandedState();
        node.RefreshPorts();

        // 3. Ϊ����title�ϴ���btn, ���btnʱ����ú���
        Button btn = new Button(() =>
        {
            AddOutputPort(node);
        });
        btn.text = "Add Output Port";
        node.titleContainer.Add(btn);

        AddElement(node);
    }

    private void AddOutputPort(DialogueNode node)
    {
        var outPort = GenPortForNode(node, Direction.Output);

        // ����node��outport����Ŀ���µ�outport����
        var count = node.outputContainer.Query("connector").ToList().Count;
        string name = $"Output {count}";
        outPort.portName = name;
        node.outputContainer.Add(outPort);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter adapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        // �̳е�GraphView���и�Property��ports, ����graph�����е�port
        ports.ForEach((port) =>
        {
            // ��ÿһ����graph���port�������жϣ���������������
            // 1. port����������������
            // 2. ͬһ���ڵ��port֮�䲻��������
            if (port != startPort && port.node != startPort.node)
            {
                compatiblePorts.Add(port);
            }
        });

        // ������⣬����������ǰ����г���startNode���port���ռ��������ŵ���List��
        // ���������������StartNode��Output port���κ�������Node��Input port������output portӦ��Ĭ�ϲ�����output port�����ɣ�
        return compatiblePorts;
    }
}
// ����dialogue graph�ĵײ�ڵ���
public class DialogueNode : Node
{
    public string GUID;
    public string Text;
    public bool Entry = false;
    //public Action<DialogueNode> onSelectAction; //ѡ�нڵ�󣬽��ڵ���Ϣ���س�ȥ

    //public DialogueNode() 
    //{
    //    this.AddManipulator(new Clickable(OnNodeSelect));
    //}

    //private void OnNodeSelect()
    //{
    //    onSelectAction?.Invoke(this);
    //}
}

