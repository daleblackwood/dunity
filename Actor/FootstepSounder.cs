using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounder : MonoBehaviour 
{
    Vector3 lastStepPos = Vector3.zero;
    Vector3 lastStepDir = Vector3.zero;
    ActorMover actorMover = null;
    bool wasGrounded = false;

    void Awake()
    {
        actorMover = GetComponent<ActorMover>();
    }

    void Update()
    {
        bool nowGrounded = actorMover.onGround;

        string soundName = "grass_walk";
        FXType fxType = FXType.None;
        if (actorMover.inWater)
        {
            soundName = "splash_sm";
            //fxType = FXType.SplashSmall;
        }

        if (nowGrounded != wasGrounded)
        {
            Step(soundName, fxType, 1.0f);
            //fxType = FXType.SplashBig;
        }
        else if (nowGrounded)
        {
            var stepDif = lastStepPos - transform.position;
            stepDif.y = 0.0f;

            bool doStep = false;

            var stepDir = transform.forward;
            stepDir.y = 0.0f;
            if (Vector3.Dot(stepDir, lastStepDir) < 0.2f)
            {
                doStep = true;
            }

            if (stepDif.sqrMagnitude > 8.0f)
            {
                doStep = true;
            }

            if (doStep)
            {
                float volume = Random.Range(0.2f, 0.3f);
                if (wasGrounded == false)
                {
                    volume = 1.0f;
                }
                Step(soundName, fxType, volume);
                lastStepPos = transform.position;
                lastStepDir = stepDir;
            }
        }

        wasGrounded = nowGrounded;
    }

    void Step(string soundName, FXType fxType = FXType.None, float volume = 1.0f)
    {
        SoundPlayer.main.Play(transform, soundName.ToString(), volume);

        if (fxType != FXType.None)
        {
            FXer.main.Fire(fxType, transform.position);
        }
    }
}
