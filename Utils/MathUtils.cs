using UnityEngine;
using System.Collections;

public static class MathUtils
{
    const float EASE_FRAME_MAGNIFY = 45.0f;

    public static float DeltaEase(float sharpness)
    {
        return Mathf.Clamp01(1.0f - Mathf.Pow(1.0f - sharpness, Time.deltaTime * EASE_FRAME_MAGNIFY));
    }

    public static float FixedDeltaEase(float sharpness)
    {
        return Mathf.Clamp01(1.0f - Mathf.Pow(1.0f - sharpness, Time.fixedDeltaTime * EASE_FRAME_MAGNIFY));
    }

    public static float PowLazy(float value, float exponent)
    {
        long tmp = System.Convert.ToInt64((double)value);
        long tmp2 = (long)(exponent * (tmp - 4606921280493453312L)) + 4606921280493453312L;
        return (float)System.Convert.ToDouble(tmp2);
    }

    public static Vector3 CatmullRom(Vector3 prev, Vector3 start, Vector3 end, Vector3 next, float elapsed, float duration = 1.0f)
    {
        var pc = elapsed / duration;
        var pcSq = pc * pc;
        var pcCb = pcSq * pc;
        return prev * (-0.5f * pcCb + pcSq - 0.5f * pc) +
            start * (1.5f * pcCb + -2.5f * pcSq + 1.0f) +
            end * (-1.5f* pcCb + 2.0f * pcSq + 0.5f * pc) +
            next * (0.5f * pcCb - 0.5f * pcSq);
    }

    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 dir, Vector3 point)
    {
        dir.Normalize();
        var linePointToPoint = point - linePoint;
        var t = Vector3.Dot(linePointToPoint, dir);
        return linePoint + dir * Mathf.Clamp01(t);
    }

    public static Vector3 ProjectPointOnAxis(Vector3 linePoint, Vector3 dir, Vector3 point)
    {
        dir.Normalize();
        var linePointToPoint = point - linePoint;
        var t = Vector3.Dot(linePointToPoint, dir);
        return linePoint + dir * t;
    }

    public static Vector3 RemoveVectorDirectionXZ(Vector3 vec, Vector3 dir)
    {
        dir.Normalize();
        var dirDot = Vector3.Dot(vec.normalized, dir);
        if (dirDot > 0.0f)
        {
            // vec = 2,2,0
            // dot = 1,0,0

            // 1. *-dot = -2,2,0 // no

            // 2. xup = 0,1,0
            // *xup = 0,2,0 // yes

            // 3.

            var cross = Vector3.Dot(dir, Vector3.down);
            vec *= cross;
        }
        return vec;
    }
}
