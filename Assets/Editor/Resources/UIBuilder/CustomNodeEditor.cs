using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CustomNodeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

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
    }
}