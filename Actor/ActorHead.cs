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
        Vector3 angles = neck.eulerAngles;
        bool wasTargeting = target != null;
        var lookDir = actor.transform.forward;

        if (target == null)
        {
            target = actor.SeekActor();
        }

        if (target != null)
        {
            var targetT = target.transform;

            var dif = targetT.position - transform.position;
            dif.y = 0.0f;
            if (dif.sqrMagnitude > 25.0f)
            {
                target = null;
            }
            else
            {
                var dot = Vector3.Dot(actor.transform.forward, dif.normalized);
                float dotThresh = wasTargeting ? -0.6f : -0.2f;
                if (dot < dotThresh)
                {
                    target = null;
                }
                else
                {
                    lookDir = dif.normalized;
                }
            }
        }

        var angleUp = Mathf.Atan2(-lookDir.z, lookDir.x) * Mathf.Rad2Deg + 180.0f;
        angles.y = angleUp;

        var rotTo = Quaternion.Euler(angles);
        var rotDifTo = Quaternion.Inverse(neck.rotation) * rotTo;
        rotDif = Quaternion.RotateTowards(rotDif, rotDifTo, Time.deltaTime * 600.0f);
        neck.rotation *= rotDif;
    }
}