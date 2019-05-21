using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Actor), true)]
public class ActorEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var actor = (Actor)target;

        DrawDefaultInspector();

        GUI.enabled = false;
        EditorGUILayout.ObjectField("Target", actor.targetActor, typeof(Actor), true);
        EditorGUILayout.ObjectField("Foe", actor.targetFoe, typeof(Actor), true);
        EditorGUILayout.ObjectField("Friend", actor.targetFriend, typeof(Actor), true);
        EditorGUILayout.ToggleLeft("Movement", actor.isMovementEnabled);
        EditorGUILayout.ToggleLeft("Down Time", actor.isDownTime);
        GUI.enabled = true;
    }
}
