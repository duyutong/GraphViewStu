using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public class BehaviorTreeView : GraphView
{
    public Action<BehaviorTreeBaseNode> onSelectAction;
    public Action onUnselectAction;
    public GameObject selectionTarget;

    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> { }
    public BehaviorTreeView()
    {
        // �����Graph����Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // ������קContent
        this.AddManipulator(new ContentDragger());
        // ������קSelection�������
        this.AddManipulator(new SelectionDragger());
        // GraphView������п�ѡ
        this.AddManipulator(new RectangleSelector());
        //�����˵�
        SearchMenuWindowProvider menu = ScriptableObject.CreateInstance<SearchMenuWindowProvider>();
        nodeCreationRequest += contentRect =>
        {
            SearchWindow.Open(new SearchWindowContext(contentRect.screenMousePosition), menu);
        };
        menu.onSelectEntryHandler += (entry, context) =>
        {
            Type type = Type.GetType(entry.name);
            return CreatNode(type);
        };
        // ���Զ����GraphViewChanged�¼����������ӵ�GraphView�¼���
        graphViewChanged += OnGraphViewChanged;
    }
    private bool CreatNode(Type type, Vector2 pos = default)
    {
        Type nodeType = Type.GetType(type.FullName);
        BehaviorTreeBaseNode node = (BehaviorTreeBaseNode)Activator.CreateInstance(nodeType);

        if (node == null) return false;

        Type stateType = GetType(node.stateName);
        node.onSelectAction = onSelectAction;
        node.onUnselected = onUnselectAction;
        node.target = selectionTarget;
        node.btState = (BehaviorTreeBaseState)Activator.CreateInstance(stateType);
        node.nodePos = pos;
        node.SetPosition(new Rect(pos, node.GetPosition().size));

        AddElement(node);
        node.RefreshExpandedState();
        node.RefreshPorts();
        return true;
    }
    private void LoadNode(NodeData nodeData)
    {
        Type nodeType = Type.GetType(nodeData.typeName);
        BehaviorTreeBaseNode node = (BehaviorTreeBaseNode)Activator.CreateInstance(nodeType);

        if (node == null) return;

        Type stateType = GetType(node.stateName);
        BehaviorTreeBaseState btState = (BehaviorTreeBaseState)Activator.CreateInstance(stateType);
        btState.InitParam(nodeData.stateParams);
        node.onSelectAction = onSelectAction;
        node.onUnselected = onUnselectAction;
        node.target = selectionTarget;
        node.btState = btState;
        node.guid = nodeData.guid;
        node.btState.output = nodeData.output;
        node.SetPosition(new Rect(nodeData.nodePos, node.GetPosition().size));

        AddElement(node);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }
    private void LoadEdge(EdgeData edgeData)
    {
        BehaviorTreeBaseNode oNode = GetBaseNode(edgeData.outPortNode);
        BehaviorTreeBaseNode iNode = GetBaseNode(edgeData.intputPortNode);
        iNode.lastNodes.Add(oNode);

        Edge edge = new Edge();
        edge.output = oNode.GetPortByName(edgeData.outPortName, Direction.Output);
        edge.input = iNode.GetPortByName(edgeData.intputPortName, Direction.Input);
        edge.input.Connect(edge);
        edge.output.Connect(edge);

        AddElement(edge);
    }
    public void LoadData(BTContainer container)
    {
        foreach (NodeData nodeData in container.nodeDatas)
            LoadNode(nodeData);
        foreach (EdgeData edgeData in container.edgeDatas)
            LoadEdge(edgeData);
    }
    private BehaviorTreeBaseNode GetBaseNode(string guid)
    {
        foreach (Node node in nodes)
        {
            BehaviorTreeBaseNode baseNode = node as BehaviorTreeBaseNode;
            if (baseNode == null) continue;
            if (baseNode.guid != guid) continue;
            return baseNode;
        }
        return null;
    }
    private Type GetType(string typeName)
    {
        string assemblyName = "Assembly-CSharp"; // ���ĳ�������
        Type type = null;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetName().Name == assemblyName)
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
        }
        return type;
    }
    public GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.elementsToRemove != null)
        {
            change.elementsToRemove.ForEach((element) =>
            {
                if (element != null)
                {
                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        BehaviorTreeBaseNode fromNode = edge.output.node as BehaviorTreeBaseNode;
                        BehaviorTreeBaseNode toNode = edge.input.node as BehaviorTreeBaseNode;
                        toNode.lastNodes.Remove(fromNode);

                        SBTOutputInfo info = new SBTOutputInfo();
                        info.fromPortName = edge.output.portName;
                        info.toPortName = edge.input.portName;
                        fromNode.btState.RefreshOutput(info, true);
                    }
                }
            });
        }
        if (change.edgesToCreate != null)
        {
            change.edgesToCreate.ForEach((edge) =>
            {
                BehaviorTreeBaseNode fromNode = edge.output.node as BehaviorTreeBaseNode;
                BehaviorTreeBaseNode toNode = edge.input.node as BehaviorTreeBaseNode;
                toNode.lastNodes.Add(fromNode);

                SBTOutputInfo info = new SBTOutputInfo();
                info.fromPortName = edge.output.portName;
                info.toPortName = edge.input.portName;
                fromNode.btState.RefreshOutput(info,false);
            });
        }
        nodes.ForEach((n) =>
        {
            BehaviorTreeBaseNode baseNode = n as BehaviorTreeBaseNode;
            baseNode.nodePos = baseNode.GetPosition().position;
        });
        return change;
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter adapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        // �̳е�GraphView���и�Property��ports, ����graph�����е�port
        ports.ForEach((endPort) =>
        {
            if (endPort == startPort) return;
            if (endPort.node == startPort.node) return;
            if (endPort.portType != startPort.portType) return;
            compatiblePorts.Add(endPort);
        });
        return compatiblePorts;
    }
}
public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
{
    public delegate bool OnSelectEntryHandler(SearchTreeEntry searchTreeEntry, SearchWindowContext context);
    public OnSelectEntryHandler onSelectEntryHandler;
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Debug.Log("CreateSearchTree");
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        entries.Add(new SearchTreeGroupEntry(new GUIContent("�����½ڵ�")));

        entries.Add(new SearchTreeGroupEntry(new GUIContent("װ����")) { level = 1 });
        List<SearchTreeEntry> decorators = GetEntries<DecoratorNode>(2);
        entries.AddRange(decorators);

        entries.Add(new SearchTreeGroupEntry(new GUIContent("������")) { level = 1 });
        List<SearchTreeEntry> triggers = GetEntries<TriggerNode>(2);
        entries.AddRange(triggers);

        entries.Add(new SearchTreeGroupEntry(new GUIContent("��Ϊ�ڵ�")) { level = 1 });
        List<SearchTreeEntry> states = GetEntries<BehaviorNode>(2);
        entries.AddRange(states);

        return entries;
    }
    private List<SearchTreeEntry> GetEntries<T>(int level)
    {
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        var nodes = GetClassList(typeof(T));
        foreach (var _node in nodes)
        {
            if (!_node.IsSubclassOf(typeof(T))) continue;
            Type type = Type.GetType(_node.FullName);
            entries.Add(new SearchTreeEntry(new GUIContent("  " + _node.FullName)) { level = 2, userData = type });
        }
        return entries;
    }
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        if (onSelectEntryHandler == null) return false;
        return onSelectEntryHandler.Invoke(searchTreeEntry, context);
    }
    private List<Type> GetClassList(Type type)
    {
        var q = type.Assembly.GetTypes()
             .Where(x => !x.IsAbstract)
             .Where(x => !x.IsGenericTypeDefinition)
             .Where(x => type.IsAssignableFrom(x));
        return q.ToList();
    }
}

