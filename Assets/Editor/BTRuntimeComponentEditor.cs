using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BTRuntimeComponent))]
public class BTRuntimeComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BTRuntimeComponent script = (BTRuntimeComponent)target;
        GUILayout.Space(10); // ���һЩ�ռ�
        if (GUILayout.Button("OpenRuntimeView", GUILayout.Height(50)))
        {
            // �����ťʱ�Ĳ���
            BehaviourTreeEditor editor = BehaviourTreeEditor.OpenWindow();
            editor.LoadExistentContainer(script.container);
        }
    }
}
