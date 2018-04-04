using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(Conversation))]
public class ConversationPropertyDrawer : PropertyDrawer
{
    ReorderableList list;
    SerializedProperty host;
    SerializedProperty talkNodes;
    Object lastObject = null;

    void Init(SerializedObject serializedObject)
    {
        host = serializedObject.FindProperty("host");
        talkNodes = serializedObject.FindProperty("talkNodes");

        list = new ReorderableList(serializedObject, talkNodes, true, true, true, true);

        var heights = new List<float>();

        list.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Conversation");
            };

        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = talkNodes.GetArrayElementAtIndex(index);

                var height = rect.height;

                var talkerRect = new Rect(rect);
                talkerRect.width *= 0.5f;

                var talkerProp = element.FindPropertyRelative("talker");
                EditorGUI.PropertyField(talkerRect, talkerProp, GUIContent.none);

                var speechRect = new Rect(rect);
                speechRect.y += EditorGUIUtility.singleLineHeight;
                speechRect.height = EditorGUIUtility.singleLineHeight * 2;

                height += speechRect.height;

                var speechProp = element.FindPropertyRelative("speech");
                speechProp.stringValue = EditorGUI.TextArea(speechRect, speechProp.stringValue);

                while (heights.Count <= index)
                {
                    heights.Add(height);
                }
            };

        list.elementHeightCallback = (index) =>
        {
                if (index < heights.Count)
                {
                    return heights[index];
                }

                return EditorGUIUtility.singleLineHeight;
        };
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (Selection.activeObject != lastObject)
        {
            lastObject = Selection.activeObject;
            Init(property.serializedObject); 
        }

        EditorGUI.BeginChangeCheck();

        list.DoLayoutList();

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}