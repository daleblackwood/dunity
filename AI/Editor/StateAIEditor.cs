using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Behave;

[CustomEditor(typeof(StateAI), true)]
public class StateAIEditor : Editor 
{
    override public void OnInspectorGUI()
    {
        var host = (StateAI)target;
        if (host.state != null)
        {
            StateEditor.Draw(host.state);
        }

        DrawDefaultInspector();
    }
}
