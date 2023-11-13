using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BTRuntimeController : MonoBehaviour
{
    private static List<BTRuntimeComponent> bTRuntimes = new List<BTRuntimeComponent>();
    public static void AddRuntime(BTRuntimeComponent bTRuntime, Action<int> action)
    {
        bTRuntimes.Add(bTRuntime);
        action(bTRuntimes.Count - 1);
    }
    public static void RemoveRuntime(int index)
    {
        bTRuntimes.RemoveAt(index);
    }
    //Test
    public void OnClickSendToTag(string _tag)
    {
        foreach (BTRuntimeComponent bTRuntime in bTRuntimes)
        {
            bTRuntime.OnReceiveMsg(_tag);
        }
    }
}
