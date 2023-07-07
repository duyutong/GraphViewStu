using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public static class GraphSaveUtility
{
    public static void SaveData(string fileName, UQueryState<Node> nodes, UQueryState<Edge> edges)
    {
        BTContainer container = ScriptableObject.CreateInstance<BTContainer>();

        foreach (Edge edge in edges)
        {
            Node outNode = edge.output.node;
            BehaviorTreeBaseNode outBaseNode = outNode as BehaviorTreeBaseNode;
            if (outBaseNode == null) continue;

            Node inputNode = edge.input.node;
            BehaviorTreeBaseNode inputBaseNode = inputNode as BehaviorTreeBaseNode;
            if (inputBaseNode == null) continue;

            EdgeData data = new EdgeData();
            data.outPortNode = outBaseNode.guid;
            data.outPortName = edge.output.portName;
            data.intputPortNode = inputBaseNode.guid;
            data.intputPortName = edge.input.portName;
            container.edgeDatas.Add(data);
        }

        foreach (Node node in nodes)
        {
            BehaviorTreeBaseNode baseNode = node as BehaviorTreeBaseNode;
            if (baseNode == null) continue;
            baseNode.btState.Save();
            NodeData data = new NodeData();
            Dictionary<string, object> _variableValues = new Dictionary<string, object>();
            FieldInfo[] fields = baseNode.btState.GetType().GetFields();
            foreach (FieldInfo field in fields) 
            {
                object value = field.GetValue(baseNode.btState);
                _variableValues.Add(field.Name, value);
            }
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(baseNode.btState.stateObj.GetType());
            string json = "";
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                jsonSerializer.WriteObject(stream, baseNode.btState.stateObj);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }
            data.lastNodes = new List<string>();
            foreach (BehaviorTreeBaseNode _node in baseNode.lastNodes) data.lastNodes.Add(_node.guid);
            data.nodeName = baseNode.title;
            data.stateParams = json;
            data.guid = baseNode.guid;
            data.nodePos = baseNode.nodePos;
            data.typeName = baseNode.GetType().FullName;
            data.output = baseNode.btState.output;
            data.stateName = baseNode.stateName;
            container.nodeDatas.Add(data);
            if (baseNode.lastNodes.Count == 0) { container.startGuid = baseNode.guid; }
        }

        AssetDatabase.CreateAsset(container, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }
}
