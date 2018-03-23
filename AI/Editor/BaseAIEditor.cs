using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Behave;

[CustomEditor(typeof(BehaveAI), true)]
public class BaseAIEditor : Editor 
{
    override public void OnInspectorGUI()
    {
        var host = (BehaveAI)target;
        BehaveEditor.Draw(host.behave);

        DrawDefaultInspector();
    }
}
