using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : BaseCam 
{
    public float distance;
    public float heightMulti;
    public float angle;
    public float zoomPower = 0.0f;

    int layerMask = 0;
    Vector3 targetLerped = Vector3.zero;
    Vector3 zoomOffset = Vector3.zero;

    protected override void UpdateCamera() 
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(targetPos + Vector3.up, Vector3.down, out hitInfo, 5.0f, layerMask))
        {
            targetPos.y = Mathf.Min(hitInfo.point.y, targetPos.y);
        }

        float rad = (angle - 90.0f) * Mathf.Deg2Rad;
        var offset = new Vector3(
            Mathf.Cos(rad) * distance,
            distance * heightMulti,
            Mathf.Sin(rad) * distance
        );

        if (zoomPower > 0.0f && targets.Length > 1)
        {
            var dif = targetBounds.size.sqrMagnitude;
            var zoomOffsetTo = offset.normalized * zoomPower * dif / cam.fieldOfView * 0.5f;
            zoomOffset = Vector3.Lerp(zoomOffset, zoomOffsetTo, MathUtils.DeltaEase(0.1f));
            offset += zoomOffset;
        }

        targetLerped = Vector3.Lerp(targetLerped, targetPos, MathUtils.DeltaEase(0.3f));
        transform.position = targetLerped + offset;
        transform.LookAt(transform.position - offset);
    }
}
