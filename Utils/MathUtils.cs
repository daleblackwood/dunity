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

    public static Vector3 VectorOnLine(Vector3 vec, Vector3 line)
    {
        var dot = Vector3.Dot(vec, line);
        if (dot <= 0.0f)
            return Vector3.zero;
        
        var lineDir = line.normalized;
        vec.x *= lineDir.x;
        vec.y *= lineDir.y;
        vec.z *= lineDir.z;

        return vec;
    }

    public static Vector3 VectorSubtractDirection(Vector3 vec, Vector3 dir)
    {
        dir.Normalize();

        var dot = Vector3.Dot(vec.normalized, dir * vec.magnitude);
        if (dot <= 0.0f)
            return vec;
        
        return vec - dir * dot;
    }
}
