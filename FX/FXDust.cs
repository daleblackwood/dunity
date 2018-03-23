using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXDust : FXInstance 
{ 
    new ParticleSystem particleSystem = null;

    void Awake()
    {
        particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    override protected void OnFire()
    {
        particleSystem.Stop();
        particleSystem.Play();
    }

    void Update()
    {
        if (timeElapsed > 0.5f)
        {
            gameObject.SetActive(false);
        }
    }
}