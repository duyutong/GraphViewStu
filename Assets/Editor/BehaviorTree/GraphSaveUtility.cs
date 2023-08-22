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
        }

        AssetDatabase.CreateAsset(container, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static void GenStateToCSharp(TempNode node) 
    {
        string tempStr = CSTemplate.stateStr;

        string stateName = node.title;
        string className = $"{stateName}State";

        tempStr = tempStr.Replace("#StateName#", stateName);

        string iStr1 = "";
        string iStr2 = "";
        string iStr3 = "";
        List<Port> iPorts = node.inputContainer.Query<Port>().ToList();
        foreach (Port port in iPorts) 
        {
            string iStrTemp1 = CSTemplate.initPropStr1;
            iStrTemp1 = iStrTemp1.Replace("#PortName#", port.portName);
            iStrTemp1 = iStrTemp1.Replace("#PortType#", port.portType.Name);
            iStr1 += iStrTemp1;

            string iStrTemp2 = CSTemplate.initPropStr2;
            iStrTemp2 = iStrTemp2.Replace("#PortName#", port.portName);
            iStr2 += iStrTemp2;

            string iStrTemp3 = CSTemplate.initPropStr3;
            iStrTemp3 = iStrTemp3.Replace("#PortName#", port.portName);
            iStr3 += iStrTemp3;
        }
        tempStr = tempStr.Replace("#PublicProperty#", iStr1);
        tempStr = tempStr.Replace("#SetPropValue#", iStr2);
        tempStr = tempStr.Replace("#SetObjPropValue#", iStr3);

        //写入文件
        string csSavePath = Application.dataPath.Replace("\\", "/") + "/Scripts/State/" + className + ".cs";
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(tempStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();

        AssetDatabase.Refresh();
        Debug.Log("状态脚本生成完毕 " + className);
    }
    public static void GenNodeToCSharp(ENodeType type, TempNode node)
    {
        string tempStr = CSTemplate.nodeStr;

        string prefix = GetPrefix(type);
        string nodeType = Enum.GetName(typeof(ENodeType), type);
        string title = node.title;

        string className = $"{prefix}_{title}";

        tempStr = tempStr.Replace("#Prefix#", prefix);
        tempStr = tempStr.Replace("#NodeType#", nodeType);
        tempStr = tempStr.Replace("#Title#", title);

        string iStr = "";
        List<Port> iPorts = node.inputContainer.Query<Port>().ToList();
        foreach (Port port in iPorts)
        {
            string iStrTemp = CSTemplate.inputContainerStr;
            iStrTemp = iStrTemp.Replace("#PortName#", port.portName);
            iStrTemp = iStrTemp.Replace("#PortType#", port.portType.Name);
            iStrTemp = iStrTemp.Replace("#Capacity#", Enum.GetName(typeof(Port.Capacity), port.capacity));
            iStr += iStrTemp;
        }

        string oStr = "";
        List<Port> oPorts = node.outputContainer.Query<Port>().ToList();
        foreach (Port port in oPorts)
        {
            string oStrTemp = CSTemplate.outputContainerStr;
            oStrTemp = oStrTemp.Replace("#PortName#", port.portName);
            oStrTemp = oStrTemp.Replace("#PortType#", port.portType.Name);
            oStrTemp = oStrTemp.Replace("#Direction#", Enum.GetName(typeof(Direction), port.direction));
            oStrTemp = oStrTemp.Replace("#Capacity#", Enum.GetName(typeof(Port.Capacity), port.capacity));
            oStr += oStrTemp;
        }

        tempStr = tempStr.Replace("#InputContainer#", iStr);
        tempStr = tempStr.Replace("#OutputContainer#", oStr);

        //写入文件
        string csSavePath = Application.dataPath.Replace("\\", "/") + "/Editor/BehaviorTree/Node/" + className + ".cs";
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(tempStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();

        AssetDatabase.Refresh();
        Debug.Log("节点脚本生成完毕 " + className);
    }
    private static string GetPrefix(ENodeType type)
    {
        switch (type)
        {
            case ENodeType.Trigger:
                return "Trigger";
            case ENodeType.Decorator:
                return "Deco";
            case ENodeType.Behavior:
                return "Behav";

        }
        return "";
    }
}
