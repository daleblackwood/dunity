using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathParent))]
public class PathParentEditor : Editor
{
    Transform selected = null;

    void OnSceneGUI() 
    {
        /*
        var pather = (PathParent)target;

        var paths = pather.GetPaths();
        for (var i=0; i<paths.Length; i++)
        {
            var path = paths[i];
            for (var j=0; j<path.Length; j++)
            {
                var node = path[j];

                //if (node == selected)
                {
                    node.position = Handles.PositionHandle(node.position, Quaternion.identity);
                }
            }
        }
        */
    }
}
