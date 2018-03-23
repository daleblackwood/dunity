using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXParticles : FXInstance
{
    ParticleSystem[] particleSystems = null;

    protected override void OnFire()
    {
        if (particleSystems == null)
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
    }
}
