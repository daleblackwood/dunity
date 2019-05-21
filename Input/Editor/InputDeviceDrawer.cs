using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(InputDevice), true)]
public class PvPropDrawer : PropertyDrawer
{
    float lineHeight = 15;
    float totalHeight = 15;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = indent;

        var buttons = property.FindPropertyRelative("pressed");

        var rect = new Rect(position);
        rect.height = lineHeight;
        for (int i=0; i<buttons.arraySize; i++)
        {
            var element = buttons.GetArrayElementAtIndex(i);
            EditorGUI.ToggleLeft(rect, ((InputDevice.ButtonType)i).ToString(), element.boolValue);
            rect.y += lineHeight;
        }

        totalHeight = rect.y;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
    {
        return totalHeight;
    }
}

