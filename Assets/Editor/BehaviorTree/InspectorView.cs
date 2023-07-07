using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public Editor editor;
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    internal void UpdateSelection(BehaviorTreeBaseNode node) 
    {
        Clear();
        Object.DestroyImmediate(editor);
        if (node.btState.stateObj == null) return;
        editor = Editor.CreateEditor(node.btState.stateObj);
        IMGUIContainer container = new IMGUIContainer(() => 
        {
            if (node == null || node.btState == null) return;
            //Debug.Log($"在Inspector中显示{node.title}内容");
            editor.OnInspectorGUI();
        });
        Add(container);
    }
}

