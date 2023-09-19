using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using BTBaseNode = BehaviorTreeBaseNode;

public class BehaviorTreeView : GraphView
{
    public Action<BTBaseNode> onSelectAction;
    public Action onUnselectAction;
    public GameObject selectionTarget;

    /// <summary>
    /// 复制对象guid和新对象guid的对比，用于新创建的edge的连接处理
    /// </summary>
    private Dictionary<string, BTBaseNode> copyNodeGuidDic = new Dictionary<string, BTBaseNode>();
    private string clipboard;

    private BTRuntimeComponent runtime;
    private Color oriColor;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, UxmlTraits> { }

    /// <summary>
    /// 构造函数，初始化BehaviorTreeView
    /// </summary>
    public BehaviorTreeView()
    {
        // 允许对Graph进行Zoom in/out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        // 允许拖拽Content
        this.AddManipulator(new ContentDragger());
        // 允许拖拽Selection里的内容
        this.AddManipulator(new SelectionDragger());
        // GraphView允许进行框选
        this.AddManipulator(new RectangleSelector());
        //搜索菜单
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

        graphViewChanged += OnGraphViewChanged;                     //处理连线生成和删除node时的相关连线清理
        serializeGraphElements += OnSerializeGraphElements;         //处理复制信息的编译
        unserializeAndPaste += OnUnserializeAndPaste;               //处理粘贴信息的反编译以及对象生成
        canPasteSerializedData += OnCanPasteSerializedData;         //处理对是否激活粘贴按钮的判断
    }

    /// <summary>
    /// 析构函数，清理资源
    /// </summary>
    ~BehaviorTreeView()
    {
        clipboard = string.Empty;

        graphViewChanged -= OnGraphViewChanged;                     //处理连线生成和删除node时的相关连线清理
        serializeGraphElements -= OnSerializeGraphElements;         //处理复制信息的编译
        unserializeAndPaste -= OnUnserializeAndPaste;               //处理粘贴信息的反编译以及对象生成
        canPasteSerializedData -= OnCanPasteSerializedData;         //处理对是否激活粘贴按钮的判断
    }

    /// <summary>
    /// 序列化Graph元素
    /// </summary>
    private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
    {
        clipboard = GraphSaveUtility.SerializeGraphElements(elements);
        return clipboard;
    }

    /// <summary>
    /// 判断是否可以粘贴已序列化的数据
    /// </summary>
    private bool OnCanPasteSerializedData(string data)
    {
        return !string.IsNullOrEmpty(clipboard);
    }

    /// <summary>
    /// 反序列化粘贴的数据并进行粘贴操作
    /// </summary>
    private void OnUnserializeAndPaste(string operationName, string data)
    {
        copyNodeGuidDic.Clear();
        ClearSelection();
        BTContainer_Copy container = GraphSaveUtility.DeserializeObject<BTContainer_Copy>(clipboard);
        foreach (NodeData nodeData in container.nodeDatas)
            PasteNode(nodeData);
        foreach (EdgeData edgeData in container.edgeDatas)
            PasteEdge(edgeData);
    }

    /// <summary>
    /// 处理粘贴边
    /// </summary>
    private void PasteEdge(EdgeData edgeData)
    {
        BTBaseNode oNode = GetCopyNode(edgeData.outPortNode);
        BTBaseNode iNode = GetCopyNode(edgeData.intputPortNode);
        iNode.lastNodes.Add(oNode);

        Edge edge = new Edge();
        edge.output = oNode.GetPortByName(edgeData.outPortName, Direction.Output);
        edge.input = iNode.GetPortByName(edgeData.intputPortName, Direction.Input);
        edge.input.Connect(edge);
        edge.output.Connect(edge);

        AddElement(edge);
        AddToSelection(edge);
    }

    /// <summary>
    /// 根据输入端口的节点的guid获取复制的节点
    /// </summary>
    private BTBaseNode GetCopyNode(string intputPortNode)
    {
        if (copyNodeGuidDic.ContainsKey(intputPortNode)) return copyNodeGuidDic[intputPortNode];
        else return null;
    }

    /// <summary>
    /// 粘贴节点
    /// </summary>
    private void PasteNode(NodeData nodeData)
    {
        BTBaseNode node = CreatCopyNode(Type.GetType(nodeData.typeName), nodeData.nodePos + 10 * Vector2.one);
        copyNodeGuidDic.Add(nodeData.guid, node);
        AddToSelection(node);
    }

    /// <summary>
    /// 处理GraphView的变化
    /// </summary>
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
                        BTBaseNode fromNode = edge.output.node as BTBaseNode;
                        BTBaseNode toNode = edge.input.node as BTBaseNode;
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
                BTBaseNode fromNode = edge.output.node as BTBaseNode;
                BTBaseNode toNode = edge.input.node as BTBaseNode;
                toNode.lastNodes.Add(fromNode);

                SBTOutputInfo info = new SBTOutputInfo();
                info.fromPortName = edge.output.portName;
                info.toPortName = edge.input.portName;
                fromNode.btState.RefreshOutput(info, false);
            });
        }
        nodes.ForEach((n) =>
        {
            BTBaseNode baseNode = n as BTBaseNode;
            baseNode.nodePos = baseNode.GetPosition().position;
        });
        return change;
    }

    /// <summary>
    /// 创建并返回复制的节点
    /// </summary>
    private BTBaseNode CreatCopyNode(Type type, Vector2 pos)
    {
        Type nodeType = type;
        BTBaseNode node = (BTBaseNode)Activator.CreateInstance(nodeType);

        if (node == null) return null;

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

        return node;
    }

    /// <summary>
    /// 创建节点
    /// </summary>
    private bool CreatNode(Type type, Vector2 pos = default)
    {
        Type nodeType = Type.GetType(type.FullName);
        BTBaseNode node = (BTBaseNode)Activator.CreateInstance(nodeType);

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

    /// <summary>
    /// 加载节点
    /// </summary>
    private void LoadNode(NodeData nodeData)
    {
        Type nodeType = Type.GetType(nodeData.typeName);
        BTBaseNode node = (BTBaseNode)Activator.CreateInstance(nodeType);

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

    /// <summary>
    /// 加载边
    /// </summary>
    private void LoadEdge(EdgeData edgeData)
    {
        BTBaseNode oNode = GetBaseNode(edgeData.outPortNode);
        BTBaseNode iNode = GetBaseNode(edgeData.intputPortNode);
        iNode.lastNodes.Add(oNode);

        Edge edge = new Edge();
        edge.output = oNode.GetPortByName(edgeData.outPortName, Direction.Output);
        edge.input = iNode.GetPortByName(edgeData.intputPortName, Direction.Input);
        edge.input.Connect(edge);
        edge.output.Connect(edge);

        AddElement(edge);
    }

    /// <summary>
    /// 加载运行时数据
    /// </summary>
    public void LoadRuntimeData(BTRuntimeComponent _runtime)
    {
        runtime = _runtime;
        selectionTarget = runtime.gameObject;
        BTContainer container = runtime.container;

        foreach (NodeData nodeData in container.nodeDatas)
            LoadRuntimNode(nodeData);
        foreach (EdgeData edgeData in container.edgeDatas)
            LoadEdge(edgeData);
    }

    /// <summary>
    /// 加载运行时节点
    /// </summary>
    private void LoadRuntimNode(NodeData nodeData)
    {
        Type nodeType = Type.GetType(nodeData.typeName);
        BTBaseNode node = (BTBaseNode)Activator.CreateInstance(nodeType);

        if (node == null) return;

        BehaviorTreeBaseState btState = runtime.stateDic[nodeData.guid];
        btState.onEnterForRuntime = () =>
        {
            node.style.backgroundColor = Color.green;
            node.RefreshExpandedState();
        };
        btState.onExitForRuntime = () =>
        {
            node.style.backgroundColor = oriColor;
            node.RefreshExpandedState();
        };

        node.onSelectAction = onSelectAction;
        node.onUnselected = onUnselectAction;
        node.target = selectionTarget;
        node.btState = btState;
        node.guid = nodeData.guid;
        node.SetPosition(new Rect(nodeData.nodePos, node.GetPosition().size));
        oriColor = node.style.backgroundColor.value;

        AddElement(node);
        node.RefreshExpandedState();
        node.RefreshPorts();
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    public void LoadData(BTContainer container)
    {
        foreach (NodeData nodeData in container.nodeDatas)
            LoadNode(nodeData);
        foreach (EdgeData edgeData in container.edgeDatas)
            LoadEdge(edgeData);
    }

    /// <summary>
    /// 根据guid获取节点
    /// </summary>
    private BTBaseNode GetBaseNode(string guid)
    {
        foreach (Node node in nodes)
        {
            BTBaseNode baseNode = node as BTBaseNode;
            if (baseNode == null) continue;
            if (baseNode.guid != guid) continue;
            return baseNode;
        }
        return null;
    }

    /// <summary>
    /// 根据类型名获取类型
    /// </summary>
    private Type GetType(string typeName)
    {
        string assemblyName = "Assembly-CSharp"; // 程序集名称
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

    /// <summary>
    /// 获取兼容的端口
    /// </summary>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter adapter)
    {
        List<Port> compatiblePorts = new List<Port>();

        // 继承的GraphView里有个Property：ports, 代表graph里所有的port
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

/// <summary>
/// 提供搜索菜单窗口的功能
/// </summary>
public class SearchMenuWindowProvider : ScriptableObject, ISearchWindowProvider
{
    /// <summary>
    /// 选择搜索结果时触发的委托
    /// </summary>
    public delegate bool OnSelectEntryHandler(SearchTreeEntry searchTreeEntry, SearchWindowContext context);

    /// <summary>
    /// 当选择搜索结果时的回调函数
    /// </summary>
    public OnSelectEntryHandler onSelectEntryHandler;

    /// <summary>
    /// 创建搜索树
    /// </summary>
    /// <param name="context">搜索窗口上下文</param>
    /// <returns>搜索树条目列表</returns>
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Debug.Log("CreateSearchTree");
        List<SearchTreeEntry> entries = new List<SearchTreeEntry>();
        entries.Add(new SearchTreeGroupEntry(new GUIContent("创建新节点")));

        entries.Add(new SearchTreeGroupEntry(new GUIContent("装饰器")) { level = 1 });
        List<SearchTreeEntry> decorators = GetEntries<DecoratorNode>(2);
        entries.AddRange(decorators);

        entries.Add(new SearchTreeGroupEntry(new GUIContent("触发器")) { level = 1 });
        List<SearchTreeEntry> triggers = GetEntries<TriggerNode>(2);
        entries.AddRange(triggers);

        entries.Add(new SearchTreeGroupEntry(new GUIContent("行为节点")) { level = 1 });
        List<SearchTreeEntry> states = GetEntries<BehaviorNode>(2);
        entries.AddRange(states);

        return entries;
    }

    /// <summary>
    /// 获取指定类型的搜索树条目列表
    /// </summary>
    /// <typeparam name="T">节点类型</typeparam>
    /// <param name="level">搜索树级别</param>
    /// <returns>搜索树条目列表</returns>
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

    /// <summary>
    /// 当选择搜索结果时调用
    /// </summary>
    /// <param name="searchTreeEntry">所选搜索树条目</param>
    /// <param name="context">搜索窗口上下文</param>
    /// <returns>是否成功处理了选择</returns>
    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        if (onSelectEntryHandler == null) return false;
        return onSelectEntryHandler.Invoke(searchTreeEntry, context);
    }

    /// <summary>
    /// 获取指定类型的类列表
    /// </summary>
    /// <param name="type">基础类型</param>
    /// <returns>类列表</returns>
    private List<Type> GetClassList(Type type)
    {
        var q = type.Assembly.GetTypes()
             .Where(x => !x.IsAbstract)
             .Where(x => !x.IsGenericTypeDefinition)
             .Where(x => type.IsAssignableFrom(x));
        return q.ToList();
    }
}

