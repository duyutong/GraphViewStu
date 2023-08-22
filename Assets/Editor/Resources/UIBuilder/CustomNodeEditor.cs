using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

public class CustomNodeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    //NodeView
    private CustomNodeView nodeView;

    //ToolBar
    private ENodeType eNodeType = ENodeType.Trigger;
    private TextField nameTextField;
    private EnumField nodeType;
    private Button createBtn;

    //PortSetting
    private VisualElement settingView;
    private ScrollView scrollView;
    private Button addPortBtn;
    private Button savePortBtn;
    private TempNode currNode;

    [MenuItem("BehaviourTreeEditor/Open NodeEditor")]
    public static void OpenWindow()
    {
        CustomNodeEditor wnd = GetWindow<CustomNodeEditor>();
        wnd.titleContent = new GUIContent("CustomNodeEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        visualTreeAsset = Resources.Load<VisualTreeAsset>("UIBuilder/CustomNodeEditor");
        visualTreeAsset.CloneTree(root);

        nodeView = root.Q<CustomNodeView>();
        nodeView.onSelectAction = OnSelectAction;
        nodeView.onUnselectAction = OnUnselectAction;

        nameTextField = root.Q<TextField>("nameTextField");

        nodeType = root.Q<EnumField>("nodeTypeField");
        nodeType.Init(eNodeType);
        nodeType.RegisterValueChangedCallback((_enum) => { eNodeType = (ENodeType)_enum.newValue; });

        createBtn = root.Q<Button>("createBtn");
        createBtn.clicked += OnClickCreateBtn;

        addPortBtn = root.Q<Button>("add");
        addPortBtn.clicked += OnClickAddPortBtn;

        savePortBtn = root.Q<Button>("save");
        savePortBtn.clicked += OnClickSaveBtn;

        scrollView = root.Q<ScrollView>("scrollView");
        settingView = root.Q<VisualElement>("settingView");
    }

    private void OnClickSaveBtn()
    {
        GraphSaveUtility.GenNodeToCSharp(eNodeType, currNode);
        GraphSaveUtility.GenStateToCSharp(currNode);
    }
    private void OnClickAddPortBtn()
    {
        ProtSettingView setting = new ProtSettingView();
        BTNodePortSetting info = new BTNodePortSetting();
        info.node = currNode;
        setting.ShowProtSetting(info,true);
        setting.onRemovePort = OnRemovePort;
        scrollView.Add(setting);
    }

    private void OnClickCreateBtn()
    {
        string nodeName = nameTextField.text;
        if (string.IsNullOrEmpty(nodeName)) return;
        nodeView.CreatNode(nodeName,eNodeType);
    }
    private void OnSelectAction(BehaviorTreeBaseNode _node) 
    {
        TempNode node = _node as TempNode;
        currNode = node;
        if (node == null) return;

        scrollView.contentContainer.Clear();
        //遍历node的所有接口
        List<Port> ports = node.inputContainer.Query<Port>().ToList();
        ports.AddRange(node.outputContainer.Query<Port>().ToList());

        foreach (Port port in ports) 
        {
            ProtSettingView setting = new ProtSettingView();
            BTNodePortSetting info = new BTNodePortSetting();
            info.node = _node;
            info.portName = port.portName;
            info.direction = port.direction;
            info.capacity = port.capacity;
            info.portType = (EPortType)Enum.Parse(typeof(EPortType), port.portType.Name);

            setting.ShowProtSetting(info);
            setting.onRemovePort = OnRemovePort;
            scrollView.Add(setting);
        }

        settingView.style.display = DisplayStyle.Flex;
    }
    private void OnRemovePort(ProtSettingView view) 
    {
        scrollView.contentContainer.Remove(view);
    }
    private void OnUnselectAction()
    {
        currNode = null;
        settingView.style.display = DisplayStyle.None;
    }
}