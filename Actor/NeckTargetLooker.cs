using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckTargetLooker : MonoBehaviour 
{
    Actor actor = null;
    Quaternion rotDif = Quaternion.identity;
    bool targeting = false;

    void Awake()
    {
        actor = GetComponentInParent<Actor>();
        if (actor == null)
        {
            enabled = false;
            return;
        }
    }

    void LateUpdate()
    {
        var angles = transform.eulerAngles;

        bool wasTargeting = targeting;
        targeting = false;

        /*
        if (actor != null || actor.targetFinder != null)
        {
            var target = actor.targetFinder.target;
            if (target != null)
            {
                var targetT = target.transform;
                if (target.castActor != null && target.castActor.face != null)
                {
                    targetT = target.castActor.face;
                }

                var dif = targetT.position - transform.position;
                var dot = Vector3.Dot(actor.transform.forward, dif.normalized);

                float dotThresh = wasTargeting ? -0.4f : -0.1f;
                if (dot > dotThresh)
                {
                    var angleUp = Mathf.Atan2(-dif.z, dif.x) * Mathf.Rad2Deg + 180.0f;

                    angles.y = angleUp;
                    targeting = true;
                }
            }
        }
        */

        var rotTo = Quaternion.Euler(angles);
        var rotDifTo = Quaternion.Inverse(transform.rotation) * rotTo;
        rotDif = Quaternion.RotateTowards(rotDif, rotDifTo, Time.deltaTime * 600.0f);

        transform.rotation *= rotDif;
    }
}
