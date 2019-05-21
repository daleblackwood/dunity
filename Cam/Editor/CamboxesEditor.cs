using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Camboxes), true)]
public class CamboxesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    void OnSceneGUI()
    {
        var cb = (Camboxes)target;

        var boxes = cb.boxes;
        if (boxes == null)
            return;

        int cid = -1;

        for (int i=0; i<boxes.Length; i++)
        {
            var box = boxes[i];

            var handleSize = 1.0f;
            if (Camera.current != null)
            {
                handleSize = (box.center - Camera.current.transform.position * 0.5f).magnitude * 0.06f;
            }

            var min = Handles.FreeMoveHandle(box.min, Quaternion.identity, handleSize, Vector3.one, Handles.SphereHandleCap);
            min.y = 0;
            box.min = min;
            var max = Handles.FreeMoveHandle(box.max, Quaternion.identity, handleSize, Vector3.one, Handles.SphereHandleCap);
            max.y = 0;
            box.max = max;
            boxes[i] = box;

            var a = box.min;
            a.y = 0;

            var b = a;
            b.x = box.max.x;

            var c = b;
            c.z = box.max.z;

            var d = c;
            d.x = a.x;

            Handles.DrawLine(a, b);
            Handles.DrawLine(b, c);
            Handles.DrawLine(c, d);
            Handles.DrawLine(d, a);

            
        }
    }
}