using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// <summary>
/// 自动生成cs文件时使用的预制文字
/// </summary>
public class CSTemplate
{
    public const string nodeStr =
        @"
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class #Prefix#_#Title# : #NodeType#
{
    public override string stateName => ""#Title#State"";
    public #Prefix#_#Title#() : base() 
    {
        title = ""#Title#"";
#InputContainer#
#OutputContainer#
    }
}
";
    public const string inputContainerStr =
        @"
        Port port_#PortName# = CreatePortForNode(this, Direction.Input, typeof(#PortType#), Port.Capacity.#Capacity#);
        port_#PortName#.portName = ""#PortName#"";
        inputContainer.Add(port_#PortName#);
";
    public const string outputContainerStr =
        @"
        Port port_#PortName# = CreatePortForNode(this, Direction.Output, typeof(#PortType#), Port.Capacity.#Capacity#);
        port_#PortName#.portName = ""#PortName#"";
        outputContainer.Add(port_#PortName#);
";
    public const string stateStr =
        @"
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class #StateName#State : BehaviorTreeBaseState
{
    #PublicProperty#

    public override BTStateObject stateObj 
    {
        get 
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<#StateName#StateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;
                #SetObjPropValue#
            }
            return _stateObj;
        }
    }
    private #StateName#StateObj _stateObj;
    public override void InitParam(string param)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(#StateName#StateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (#StateName#StateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;
            #SetPropValue#
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;
        #SetPropValue#
    }
}
public class #StateName#StateObj : BTStateObject
{
    public EBTState state;
    #PublicProperty#

    public List<BTOutputInfo> output;
}
";
    public const string initPropStr1 = @"
public #PortType# #PortName#;";
    public const string initPropStr2 = @"
#PortName# = _stateObj.#PortName#;";
    public const string initPropStr3 = @"
_stateObj.#PortName# = #PortName#;";
}
