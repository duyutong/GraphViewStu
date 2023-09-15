using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class OpenEditorWindowOnStartup
{
    static OpenEditorWindowOnStartup()
    {
        // 注册Unity编辑器启动时的回调
        EditorApplication.delayCall += OpenWindow;
    }

    static void OpenWindow()
    {
        // 打开你的编辑器窗口
        ClockoutCountdown window = EditorWindow.GetWindow<ClockoutCountdown>();
        window.Show();

        // 取消回调注册，确保只在启动时打开一次窗口
        EditorApplication.delayCall -= OpenWindow;
    }
}
