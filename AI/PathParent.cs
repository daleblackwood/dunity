using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PathParent : GameBehaviour 
{
    public void OnDrawGizmos()
    {
        #if UNITY_EDITOR

        if (Selection.activeGameObject == null)
            return;

        if (Selection.activeGameObject.transform.IsChildOf(transform) == false)
            return;

        var prevColor = Gizmos.color;
        var paths = GetPaths();
        for (var i=0; i<paths.Length; i++)
        {
            var path = paths[i];
            var parent = path[0].parent;
            float colorI = parent.GetSiblingIndex();
            Gizmos.color = Color.HSVToRGB(Mathf.Repeat(colorI * 0.28f, 1.0f), 0.8f, 1.0f);

            Vector3 lastPos = Vector3.zero;

            for (var j=0; j<path.Length; j++)
            {
                var node = path[j];
                var pos = node.position;

                Gizmos.DrawSphere(pos, 0.4f);

                if (lastPos == Vector3.zero) 
                {
                    lastPos = pos;
                }
                else
                {
                    Gizmos.DrawLine(lastPos, pos);
                }

                lastPos = pos;
            }
        }
        Gizmos.color = prevColor;

        #endif
    }

    public Transform[][] GetPaths()
    {
        var pathList = new List<Transform[]>();
        for (var i=0; i<transform.childCount; i++)
        {
            var childList = new List<Transform>();
            var nodeParent = transform.GetChild(i);
            if (nodeParent.gameObject.activeSelf == false)
                continue;
            
            for (var j=0; j<nodeParent.childCount; j++)
            {
                var node = nodeParent.GetChild(j);
                if (node.gameObject.activeSelf == false)
                    continue;

                childList.Add(node);
            } 

            if (childList.Count < 1)
                continue;
            
            pathList.Add(childList.ToArray());
        }
        return pathList.ToArray();
    }
}
