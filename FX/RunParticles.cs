using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunParticles : MonoBehaviour 
{
    ParticleSystem particles = null;
    ActorMover mover = null;

    bool emitting = false;

    void OnEnable()
    {
        if (particles == null)
        {
            particles = GetComponent<ParticleSystem>();
            mover = GetComponentInParent<ActorMover>();
        }
    }

    void OnDisable()
    {
        SetEmitting(false);
    }

    void LateUpdate()
    {
        SetEmitting(mover.onGround);
    }

    void SetEmitting(bool enable)
    {
        if (emitting == enable)
            return;
        
        emitting = enable;

        if (enable)
        {
            particles.Play();
            particles.Emit(3);
        }
        else
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
