using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TalkTrigger), true)]
public class TalkTriggerEditor : Editor 
{
    GUIStyle styleSpeechText = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var talkNodes = serializedObject.FindProperty("talkNodes");
        DwilEditorLayout.ArrayField("Conversation", talkNodes, (Rect rect, int index, SerializedProperty prop) =>
            {
                var hostProp = prop.FindPropertyRelative("actor");
                var talkerProp = prop.FindPropertyRelative("talker");
                if (talkerProp.enumValueIndex == (int)Talker.Custom)
                {
                    bool wasEmpty = hostProp.objectReferenceValue == null;
                    EditorGUI.ObjectField(rect, hostProp, GUIContent.none);
                    if (hostProp.objectReferenceValue == null && wasEmpty == false)
                    {
                        talkerProp.enumValueIndex = (int)Talker.Host;
                    }
                }
                else
                {
                    EditorGUI.PropertyField(rect, talkerProp, GUIContent.none);
                }

                //if (styleSpeechText == null)
                {
                    styleSpeechText = new GUIStyle(EditorStyles.textField);
                    styleSpeechText.wordWrap = true;
                    styleSpeechText.fontSize = 11;
                }
                var speechRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight * 2);
                speechRect.x = rect.x;
                speechRect.width -= rect.x;
                var speechProp = prop.FindPropertyRelative("speech");
                speechProp.stringValue = EditorGUI.TextArea(speechRect, speechProp.stringValue, styleSpeechText);

                var triggerRect = EditorGUILayout.GetControlRect();
                triggerRect.x = speechRect.x;
                triggerRect.width = speechRect.width;
                var triggerProp = prop.FindPropertyRelative("trigger");
                EditorGUI.PropertyField(triggerRect, triggerProp, new GUIContent("...then trigger"));

                EditorGUILayout.GetControlRect(false, 4);
            });
    }
}
