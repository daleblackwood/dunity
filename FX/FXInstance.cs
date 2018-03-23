using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FXInstance : MonoBehaviour 
{
    public float timeElapsed { get { return Time.time - timeStart; } }
    public float timeElapsedUnscaled { get { return Time.unscaledTime - timeStartUnscaled; } }
    public float timeStart { get; private set; }
    public float timeStartUnscaled { get; private set; }
    public int frameStart { get; private set; }
    public int framesElapsed { get { return Time.frameCount - frameStart; } }

    public void CallFire()
    {
        timeStart = Time.time;
        timeStartUnscaled = Time.unscaledTime;
        frameStart = Time.frameCount;
        OnFire();
    }

    protected virtual void OnFire() {}
}
