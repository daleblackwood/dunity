using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHead : MonoBehaviour 
{
    public Actor target { get; private set; }
    Actor actor = null;
    Quaternion initRot;
    Transform neck;
    Quaternion neckInitRot;
    Quaternion rotDif = Quaternion.identity;
    Quaternion neckRot = Quaternion.identity;
    public Vector3 baseAngles;
    public bool autoUpdate = false;

    void Awake()
    {
        actor = GetComponentInParent<Actor>();
        initRot = transform.localRotation;
        neck = transform.parent;
        neckInitRot = neck.localRotation;
    }

    void LateUpdate()
    {
        transform.localRotation = initRot;
        UpdateNeck();
    }

    void UpdateNeck()
    {
        bool wasTargeting = target != null;
        var lookDir = actor.transform.forward;
        var bodyDir = lookDir;

        if (autoUpdate || target == null)
        {
            target = actor.SeekContextActor();
        }

        target = actor.targetActor;

        if (target != null)
        {
            var dif = target.transform.position - transform.position;
            dif.y = 0.0f;
            lookDir = dif.normalized;
        }

        /*//
        float lookAngle = DirToAngle(lookDir);
        float maxAngle = 89.0f;
        float lookAngleTo = Mathf.Clamp(lookAngle, bodyAngle - maxAngle, bodyAngle + maxAngle);
        float lookDif = Mathf.Repeat(lookAngleTo - headAngle + 90.0f, 180.0f) - 90.0f;
        float lookDelta = Time.deltaTime * 600.0f;
        lookAngle = lookAngleTo + Mathf.Clamp(lookDif, -lookDelta, lookDelta);
        angles.y = lookAngle;
        var rotTo = Quaternion.Euler(angles);
        /*/
        var rotOffset = Quaternion.Euler(0, 90, -80);
        var bodyRot = Quaternion.LookRotation(bodyDir, Vector3.up);
        var lookRot = Quaternion.LookRotation(lookDir, Vector3.up);
        lookRot = Quaternion.RotateTowards(bodyRot, lookRot, 90.0f);
        var rotTo = lookRot * rotOffset;
        neckRot = Quaternion.RotateTowards(neckRot, rotTo, Time.deltaTime * 600.0f);

        //*/
        neck.rotation = neckRot;
    }

    float DirToAngle(Vector3 dir)
    {
        float angle = Mathf.Atan2(-dir.z, dir.x) * Mathf.Rad2Deg + 180.0f;
        return WrapAngle(angle);
    }

    float WrapAngle(float angle)
    {
        return Mathf.Repeat(angle + 180.0f, 360.0f) - 180.0f;
    }
}