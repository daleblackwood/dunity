using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Behave
{
    public static class BehaveEditor
    {
        static bool show = true;
        static Dictionary<int, Texture> colorTexs = new Dictionary<int, Texture>();

        static public void Draw(BNode node)
        {
            var rect = EditorGUILayout.GetControlRect();
            var hasNodes = node != null;
            var str = hasNodes ? "Behave " + node.ToString() : "Behave inactive";
            show = EditorGUI.Foldout(rect, show, str);
            rect.y += EditorGUIUtility.singleLineHeight;
            rect.height += EditorGUIUtility.singleLineHeight;
            
            if (show && node != null)
            {
                DrawNode(node);
            }
        }

        static void DrawNode(BNode node, int level = 0, bool active = true)
        {
            var rect = EditorGUILayout.GetControlRect();

            if (node == null)
            {
                EditorGUILayout.LabelField("Behaviour empty.");
                return;
            }

            active = active && node.result == BResult.Ticking;

            var style = active ? EditorStyles.boldLabel : EditorStyles.label;
            
            float statusWidth = 12.0f;
            float statusesWidth = statusWidth * 2.0f;
            float countWidth = 20.0f;
            float labelWidth = rect.width - statusesWidth - countWidth;

            var str = "";
            for (int i=0; i<level; i++) {
                if (i == level - 1) 
                {
                    str += active ? "+ " : "- ";
                }
                else
                {
                    str += "   ";
                }
            }
            str += node.ToString();
            var subrect = new Rect(rect.x, rect.y, labelWidth, rect.height);
            EditorGUI.LabelField(subrect, str, style);

            subrect.width = countWidth;
            subrect.x = labelWidth;
            str = node.runCount.ToString();
            EditorGUI.LabelField(subrect, str);

            subrect.x += countWidth;
            if (node.result != BResult.Beginning)
            {
                var color = Color.black;
                if (node.result == BResult.Ticking)
                {
                    color = Color.green;
                }
                else if (node.result == BResult.Failed)
                {
                    color = Color.Lerp(Color.red, Color.black, 0.5f);
                }
                else if (node.result == BResult.Succeeded)
                {
                    color = Color.Lerp(Color.green, Color.black, 0.5f);
                }
                var statusRect = new Rect(subrect.x + 2, subrect.y + 2, statusWidth - 4, statusWidth - 4);
                EditorGUI.DrawRect(statusRect, color);

                statusRect.x += statusWidth;
                
                if (active)
                {
                    if (GUI.Button(statusRect, node.runCount.ToString()))
                    {
                        node.Cancel();
                    }
                }
            }

            if (node is BParent)
            {
                var parent = (BParent)node;
                var subnodes = parent.GetChildren();
                if (subnodes != null && subnodes.Length > 0) 
                {
                    foreach (var subnode in subnodes)
                    {
                        rect.y += rect.height;
                        rect.height += rect.height;
                        DrawNode(subnode, level + 1, active);
                    }
                }
            }
        }
    }
}