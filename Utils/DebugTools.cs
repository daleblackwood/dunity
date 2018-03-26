using UnityEngine;

public static class DebugTools
{
    static public void DrawCircle(Vector3 pos, Color color, float diameter = 2.0f)
    {
        Vector3 lastPos = Vector3.zero;
        float sep = Mathf.PI * 0.1f;
        float startAng = Mathf.Repeat(Time.time, Mathf.PI * 2.0f);
        for (float ang = 0.0f; ang < Mathf.PI * 2.0f; ang += sep)
        {
            var nextPos = pos + new Vector3(Mathf.Cos(ang + startAng), 0.0f, Mathf.Sin(ang + startAng)) * diameter;
            if (ang > 0.0f)
            {
                Debug.DrawLine(nextPos, lastPos, color);
                Debug.DrawLine(pos, nextPos, color);
            }
            lastPos = nextPos;
        }
    }

    static public void DrawCircleFacing(Vector3 pos, Color color, float diameter = 1.0f)
    {
        var cam = Camera.main;
        if (cam == null)
            return;
        
        Vector3 forward = Camera.main.transform.forward;

        Vector3 lastPos = Vector3.zero;
        float sep = 20.0f;
        float startAng = Mathf.Repeat(Time.time, Mathf.PI * 2.0f);
        for (float ang = 0.0f; ang < 360.0f; ang += sep)
        {
            float thisAng = startAng + ang;
            var q = Quaternion.AngleAxis(thisAng, forward) * Vector3.up;
            var a = pos + q;
            var b = pos - q;

            var nextPos = a;
            if (ang > 0.0f)
            {
                Debug.DrawLine(nextPos, lastPos, color);
                Debug.DrawLine(pos, nextPos, color);
            }
            lastPos = nextPos;
        }
    }

    static public void DrawSphere(Vector3 position, Color color, float radius = 1.0f)
    {
        float sep = 360.0f / 16.0f;
        for (float ang = 0; ang < 360.0f + sep; ang += sep)
        {
            Vector3 axis = Quaternion.AngleAxis(ang, Vector3.up) * Vector3.right;
            DrawCircleAxis(position, axis, color, radius);
        }
    }

    static public void DrawCircleAxis(Vector3 position, Vector3 axis, Color color, float radius = 1.0f)
    {
        float sep = 360.0f / 16.0f;
        Vector3 lastPos = Vector3.zero;
        for (float ang = 0; ang < 360.0f + sep; ang += sep)
        {
            var p = position + Quaternion.AngleAxis(ang, axis) * new Vector3(radius, 0, 0);
            if (ang > float.Epsilon)
            {
                Debug.DrawLine(lastPos, p, color);
            }
            lastPos = p;
        }
    }
}