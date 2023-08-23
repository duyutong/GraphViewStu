using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GridBackView : GraphView
{
    public new class UxmlFactory : UxmlFactory<GridBackView, UxmlTraits> { }
    public GridBackView() 
    {
        Insert(0, new GridBackground());
        //添加背景网格样式
        StyleSheet styleSheet = Resources.Load<StyleSheet>("UIBuilder/BehaviorTree/BehaviourTreeEditor");
        styleSheets.Add(styleSheet);
    }
}
