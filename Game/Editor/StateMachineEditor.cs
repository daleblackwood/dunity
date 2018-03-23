using UnityEngine;
using UnityEditor;

public static class StateEditor
{
    static bool show = true;

    static public void Draw(StateMachine stateMachine)
    {
        DrawStateInfo(stateMachine.stateCurrent);
        for (int i=0; i<stateMachine.stack.Length; i++)
        {
            DrawStateInfo(stateMachine.stack[i]);
        }
    }

    static void DrawStateInfo(StateInfo stateInfo)
    {
        var rect = EditorGUILayout.GetControlRect();

        if (stateInfo == null)
        {
            EditorGUILayout.LabelField("State empty.");
            return;
        }

        bool active = stateInfo.isComplete == false;

        var style = active ? EditorStyles.boldLabel : EditorStyles.label;

        float statusWidth = 12.0f;
        float statusesWidth = statusWidth * 2.0f;
        float countWidth = 20.0f;
        float labelWidth = rect.width - statusesWidth - countWidth;

        var str = "";
        str += stateInfo.name;
        var subrect = new Rect(rect.x, rect.y, labelWidth, rect.height);
        EditorGUI.LabelField(subrect, str, style);

        subrect.width = countWidth;
        subrect.x = labelWidth;
        str = stateInfo.time.ToString("0.0");
        EditorGUI.LabelField(subrect, str);

        subrect.x += countWidth;
        if (stateInfo.status != StateInfo.Status.Init)
        {
            var color = Color.black;
            if (stateInfo.status == StateInfo.Status.Running)
            {
                color = Color.green;
            }
            else if (stateInfo.status == StateInfo.Status.CompleteDefault)
            {
                color = Color.Lerp(Color.red, Color.black, 0.5f);
            }
            else
            {
                color = Color.Lerp(Color.green, Color.black, 0.5f);
            }
            var statusRect = new Rect(subrect.x + 2, subrect.y + 2, statusWidth - 4, statusWidth - 4);
            EditorGUI.DrawRect(statusRect, color);

            statusRect.x += statusWidth;

            /*
            if (active)
            {
                if (GUI.Button(statusRect, node.runCount.ToString()))
                {
                    node.Cancel();
                }
            }
            */
        }
    }
}
