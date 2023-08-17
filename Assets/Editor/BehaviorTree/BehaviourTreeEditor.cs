using System;
using System.ComponentModel;
using Unity.Plastic.Antlr3.Runtime.Tree;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;
public class BehaviourTreeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    private InspectorView inspectorView;
    private BehaviorTreeView behaviorTreeView;
    private Button saveBtn;
    private Button loadBtn;
    private TextField nameTextField;
    private ObjectField treeField;

    [MenuItem("BehaviourTreeEditor/Open Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        visualTreeAsset = Resources.Load<VisualTreeAsset>("UIBuilder/BehaviourTreeEditor");
        visualTreeAsset.CloneTree(root);

        inspectorView = root.Q<InspectorView>();
        behaviorTreeView = root.Q<BehaviorTreeView>();

        behaviorTreeView.onSelectAction = OnSelectAction;
        behaviorTreeView.selectionTarget = Selection.activeGameObject;

        saveBtn = root.Q<Button>("saveBtn");
        saveBtn.clicked += OnClickSaveBtn;

        loadBtn = root.Q<Button>("loadBtn");
        loadBtn.clicked += OnClickLoadBtn;

        treeField = root.Q<ObjectField>("treeField");
        nameTextField = root.Q<TextField>("nameTextField");
    }
    private void OnClickLoadBtn() 
    {
        BTContainer container = treeField.value as BTContainer;
        if (container == null) return;
        if (container.nodeDatas.Count == 0) Debug.Log("没有数据！");
        nameTextField.value = container.name;
        behaviorTreeView.LoadData(container);
    }
    private void OnClickSaveBtn()
    {
        GraphSaveUtility.SaveData(nameTextField.text, behaviorTreeView.nodes, behaviorTreeView.edges);
    }
    private void OnSelectAction(BehaviorTreeBaseNode node)
    {
        //Debug.Log("BehaviourTreeEditor -> " + node.title);
        inspectorView.UpdateSelection(node);
    }
}
