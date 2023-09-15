using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class OpenEditorWindowOnStartup
{
    static OpenEditorWindowOnStartup()
    {
        // ע��Unity�༭������ʱ�Ļص�
        EditorApplication.delayCall += OpenWindow;
    }

    static void OpenWindow()
    {
        // ����ı༭������
        ClockoutCountdown window = EditorWindow.GetWindow<ClockoutCountdown>();
        window.Show();

        // ȡ���ص�ע�ᣬȷ��ֻ������ʱ��һ�δ���
        EditorApplication.delayCall -= OpenWindow;
    }
}
