using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class DwilEditorLayout
{
    static GUIStyle styleListButton = null;
    static GUIStyle styleListArrowButton = null;

    static public List<T> ListField<T>(string title, List<T> list, System.Action<Rect, int> guiFunc, int min = 0, int max = int.MaxValue)
    {
        if (list == null)
        {
            list = new List<T>();
        }

        GUILayout.BeginVertical(EditorStyles.helpBox);

        int bw = 20;

        title = title + " (" + list.Count.ToString();
        if (max < int.MaxValue)
        {
            title += " of " + max.ToString();
        }
        title += ")";

        if (styleListButton == null)
        {
            styleListButton = new GUIStyle(GUI.skin.button);
            styleListButton.margin = new RectOffset(-2,0,-3,0);
            styleListButton.fontStyle = FontStyle.Bold;
            styleListButton.fontSize = 13;
            styleListButton.contentOffset = Vector2.zero;
        }

        if (styleListArrowButton == null)
        {
            styleListArrowButton = new GUIStyle(GUI.skin.button);
            styleListArrowButton.fontSize = 9;
        }

        var labelRect = EditorGUILayout.GetControlRect();
        labelRect.width -= 3;
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        var br = new Rect(labelRect);
        br.width = bw;
        br.x = labelRect.width;
        if (list.Count < max - 1 && GUI.Button(br, "+"))
        {
            list.Add(list.Count > 0 ? list[list.Count - 1] : default(T));
        }

        int remove = -1;

        var bottomRect = labelRect;

        for (int i=0; i<list.Count; i++)
        {
            var arrayRect = EditorGUILayout.GetControlRect();
            bottomRect = arrayRect;
            arrayRect.x += 10;
            arrayRect.width -= 1;

            var sr = new Rect(arrayRect);
            sr.width -= 80;

            var lr = new Rect(sr);
            lr.width = 15;
            EditorGUI.LabelField(lr, i.ToString() + ".");
            sr.x += lr.width;
            sr.width -= lr.width;

            guiFunc(sr, i);

            int moving = 0;

            br = new Rect(arrayRect);
            br.x = sr.x + sr.width;
            br.width = bw;
            if (i > 0 && GUI.Button(br, "▲", styleListArrowButton))
            {
                moving = -1;
            }

            br.x += bw;

            if (i < list.Count - 1 && GUI.Button(br, "▼", styleListArrowButton))
            {
                moving = 1;
            }

            br.x += bw * 1.5f;
            if (list.Count > min && GUI.Button(br, "×", styleListButton))
            {
                remove = i;
            }

            if (moving != 0)
            {
                int newIndex = Mathf.Clamp(i + moving, 0, list.Count - 1);
                var temp = list[newIndex];
                list[newIndex] = list[i];
                list[i] = temp;
            }
        }

        if (remove >= 0)
        {
            list.RemoveAt(remove);
        }

        while (list.Count < min)
        {
            list.Add(default(T));
        }
        if (list.Count > max)
        {
            list.RemoveRange(max, list.Count - max);
        }

        GUILayout.EndVertical();

        return list;
    }

    static public void ArrayField(string title, SerializedProperty arrayProp, System.Action<Rect, int, SerializedProperty> guiFunc, int min = 0, int max = int.MaxValue)
    {
        GUILayout.BeginVertical(EditorStyles.helpBox);

        if (arrayProp.isArray == false)
        {
            EditorGUILayout.HelpBox("Property isn't an array.", MessageType.Error);
            return;
        }

        int bw = 20;

        title = title + " (" + arrayProp.arraySize.ToString();
        if (max < int.MaxValue)
        {
            title += " of " + max.ToString();
        }
        title += ")";

        if (styleListButton == null)
        {
            styleListButton = new GUIStyle(GUI.skin.button);
            styleListButton.margin = new RectOffset(-2,0,-3,0);
            styleListButton.fontStyle = FontStyle.Bold;
            styleListButton.fontSize = 13;
            styleListButton.contentOffset = Vector2.zero;
        }

        if (styleListArrowButton == null)
        {
            styleListArrowButton = new GUIStyle(GUI.skin.button);
            styleListArrowButton.fontSize = 9;
        }

        var labelRect = EditorGUILayout.GetControlRect();
        labelRect.width -= 3;
        EditorGUI.LabelField(labelRect, title, EditorStyles.boldLabel);

        var br = new Rect(labelRect);
        br.width = bw;
        br.x = labelRect.width;

        if (arrayProp.arraySize < max - 1 && GUI.Button(br, "+"))
        {
            //arrayProp.arraySize = arrayProp.arraySize > 0 ? arrayProp.GetArrayElementAtIndex(arrayProp.arraySize - 1) : default(T);
            arrayProp.arraySize++;
        }

        int remove = -1;
        var bottomRect = labelRect;

        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            var arrayRect = EditorGUILayout.GetControlRect();
            bottomRect = arrayRect;
            arrayRect.x += 10;
            arrayRect.width -= 1;

            var sr = new Rect(arrayRect);
            sr.width -= 80;

            var lr = new Rect(sr);
            lr.width = 15;
            EditorGUI.LabelField(lr, i.ToString() + ".");
            sr.x += lr.width;
            sr.width -= lr.width;

            guiFunc(sr, i, arrayProp.GetArrayElementAtIndex(i));

            int moving = 0;

            br = new Rect(arrayRect);
            br.x = sr.x + sr.width;
            br.width = bw;
            if (i > 0 && GUI.Button(br, "▲", styleListArrowButton))
            {
                moving = -1;
            }

            br.x += bw;

            if (i < arrayProp.arraySize - 1 && GUI.Button(br, "▼", styleListArrowButton))
            {
                moving = 1;
            }

            br.x += bw * 1.5f;
            if (arrayProp.arraySize > min && GUI.Button(br, "×", styleListButton))
            {
                remove = i;
            }

            if (moving != 0)
            {
                int newIndex = Mathf.Clamp(i + moving, 0, arrayProp.arraySize - 1);
                var temp = arrayProp.GetArrayElementAtIndex(newIndex);
                arrayProp.MoveArrayElement(i, newIndex);
            }
        }

        if (remove >= 0)
        {
            arrayProp.DeleteArrayElementAtIndex(remove);
        }

        while (arrayProp.arraySize < min)
        {
            arrayProp.arraySize++;
        }
        if (arrayProp.arraySize > max)
        {
            arrayProp.DeleteArrayElementAtIndex(arrayProp.arraySize - 1);
        }

        arrayProp.serializedObject.ApplyModifiedProperties();

        GUILayout.EndVertical();
    }
}