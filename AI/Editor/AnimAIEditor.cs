using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AnimAI))]
public class AnimAIEditor : Editor
{
    override public void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}