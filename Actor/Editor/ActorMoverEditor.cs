using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ActorMover), true)]
public class ActorMoverEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var mover = (ActorMover)target;

        DrawDefaultInspector();

        EditorGUILayout.LabelField(mover.onGround ? "On Ground" : "In Air", mover.onWall ? "On Wall" : "Off Wall");
    }
}
