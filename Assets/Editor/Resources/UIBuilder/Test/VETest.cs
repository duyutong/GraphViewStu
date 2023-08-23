using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class VETest : EditorWindow
{
    private VisualTreeAsset visualTreeAsset = default;
    private DropdownField dropdownField;
    [MenuItem("Tool/VETest")]
    public static void ShowExample()
    {
        VETest wnd = GetWindow<VETest>();
        wnd.titleContent = new GUIContent("VETest");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        visualTreeAsset = Resources.Load<VisualTreeAsset>("UIBuilder/Test/VETest");
        visualTreeAsset.CloneTree(root);

        dropdownField = root.Q<DropdownField>();
        dropdownField.choices = new List<string>() { "Test1", "Test2" };
        dropdownField.value = "Test1";
        dropdownField.RegisterValueChangedCallback((_str) => { Debug.Log(_str.newValue); });
    }
}