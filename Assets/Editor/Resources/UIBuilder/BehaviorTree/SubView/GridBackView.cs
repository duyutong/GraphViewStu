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
        //��ӱ���������ʽ
        StyleSheet styleSheet = Resources.Load<StyleSheet>("UIBuilder/BehaviorTree/BehaviourTreeEditor");
        styleSheets.Add(styleSheet);
    }
}
