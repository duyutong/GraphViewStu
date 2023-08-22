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
        GUILayout.Space(10); // 添加一些空间
        if (GUILayout.Button("OpenRuntimeView", GUILayout.Height(50)))
        {
            // 点击按钮时的操作
            BehaviourTreeEditor editor = BehaviourTreeEditor.OpenWindow();
            editor.LoadExistentContainer(script.container);
        }
    }
}
