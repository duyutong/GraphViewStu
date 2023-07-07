using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// �������GraphView���Canvas��EditorWindow
public class DialogueGraphWindow : EditorWindow
{
    private DialogueGraphView _graphView;
    private string _fileName = "New Narrative";

    // ͨ��Menu���ɴ򿪶�Ӧwindow, ע�����ֺ���������static����
    [MenuItem("Graph/Open Dialogue Graph View")]
    public static void OpenDialogueGraphWindow()
    {
        // �����˴�������Window�ķ���
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

        // ��graphView��������Editor����
        _graphView.StretchToParentSize();
        // ������ӵ�EditorWindow�Ŀ��ӻ�RootԪ������
        rootVisualElement.Add(_graphView);

        //  ��������漰���˵����ã�����Ӧ�÷ŵ�DialogueGraphWindow����
        // ���Toolbar����UnityEditor.UIElements��
        Toolbar toolbar = new Toolbar();
        //����lambda��������������ť�����ĺ�������
        Button btn = new Button(clickEvent: () => { _graphView.AddDialogueNode("Dialogue"); });
        btn.text = "Add Dialogue Node";
        toolbar.Add(btn);
        rootVisualElement.Add(toolbar);

        // ���TextField
        TextField fileNameTextField = new TextField(label: "File Name");
        fileNameTextField.SetValueWithoutNotify(_fileName);// ����˽�г�Ա_fileName = "New Narrative";
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        // �����Button
        // LodaData��SaveData����������ʱ��ûʵ��
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

    // �رմ���ʱ����graphView
    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
