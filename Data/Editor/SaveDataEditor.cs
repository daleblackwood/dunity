using UnityEngine;
using UnityEditor;
using SimpleJSON;

[CustomEditor(typeof(SaveData))]
public class SaveDataEditor : Editor
{
    int lastSaveNo = -1;
    string saveText;

    override public void OnInspectorGUI()
    {
        var save = (SaveData)target;

        if (lastSaveNo != SaveData.saveCount)
        {
            lastSaveNo = SaveData.saveCount;
            saveText = StringNode(save.game.data);
        }

        GUI.enabled = false;
        EditorGUILayout.TextArea(saveText);
        GUI.enabled = true;
    }

    string StringNode(JSONNode node, int indent = 0)
    {
        var result = "";

        var prefix = "";
        for (int i=0; i<indent; i++)
        {
            prefix += " ";
        }

        foreach (var subnode in node)
        {
            result += prefix + subnode.Key + ": ";
            if (subnode.Value.IsObject)
            {
                result += "\n" + StringNode(subnode, indent + 3);
            }
            else
            {
                result += subnode.Value.ToString() + "\n";
            }
        }

        return result;
    }
}