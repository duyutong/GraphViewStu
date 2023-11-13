using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class GridBackView : GraphView
{
    public new class UxmlFactory : UxmlFactory<GridBackView, UxmlTraits> { }
    public GridBackView() 
    {
        Insert(0, new GridBackground());
        //添加背景网格样式
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UIBuilder/BehaviorTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
}
