using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// 代表放置GraphView这个Canvas的EditorWindow
public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    // 通过Menu即可打开对应window, 注意这种函数必须是static函数
    [MenuItem("Graph/Open Dialogue Graph View")]
    public static void OpenDialogueGraphWindow()
    {
        // 定义了创建并打开Window的方法
        var window = GetWindow<DialogueGraphWindow>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }
    private void OnEnable()
    {
        Debug.Log("New GraphView");
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        // 让graphView铺满整个Editor窗口
        _graphView.StretchToParentSize();
        // 把它添加到EditorWindow的可视化Root元素下面
        rootVisualElement.Add(_graphView);

        //  相关内容涉及到菜单设置，所以应该放到DialogueGraphWindow类下
        // 这个Toolbar类在UnityEditor.UIElements下
        Toolbar toolbar = new Toolbar();
        //创建lambda函数，代表点击按钮后发生的函数调用
        Button btn = new Button(clickEvent: () => { _graphView.AddDialogueNode("Dialogue"); });
        btn.text = "Add Dialogue Node";
        toolbar.Add(btn);
        rootVisualElement.Add(toolbar);

        // 添加TextField
        TextField fileNameTextField = new TextField(label: "File Name");
        fileNameTextField.SetValueWithoutNotify(_fileName);// 类内私有成员_fileName = "New Narrative";
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        // 添加两Button
        // LodaData和SaveData两个函数暂时还没实现
        toolbar.Add(new Button(() => SaveData()) { text = "Save Data" });
        toolbar.Add(new Button(() => LoadData()) { text = "Load Data" });
        toolbar.Add(new Button(() => ClearData()) { text = "Clear Data" });
    }

    private void ClearData()
    {
        _graphView.DeleteElements(_graphView.nodes);
        _graphView.DeleteElements(_graphView.edges);
    }

    private void LoadData()
    {

    }

    private void SaveData()
    {

    }

    // 关闭窗口时销毁graphView
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
