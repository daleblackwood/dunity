using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour 
{
    static public TimeManager main { get; private set; }

    public float defaultTimeScale = 1.0f;
    float initFixedDelta = 0.02f;
    float lastDefaultTimeScale = 0.0f;

    void Awake()
    {
        main = this;
        initFixedDelta = Time.fixedDeltaTime;
    }

    void Update()
    {
        if (defaultTimeScale != lastDefaultTimeScale)
        {
            Reset();
            lastDefaultTimeScale = defaultTimeScale;
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        SetScale(defaultTimeScale);
    }

    public void Freeze()
    {
        StopAllCoroutines();
        SetScale(0.0f);
    }

    public void SlowMo(float speed = 0.3f)
    {
        StopAllCoroutines();
        SetScale(speed);
    }

    public void FrameFreeze(int frames = 6)
    {
        if (frames < 1)
            return;
        
        Reset();
        StartCoroutine(CoFrameFreeze(frames));
    }

    IEnumerator CoFrameFreeze(int frames)
    {
        SetScale(0.0f);

        while (frames-- > 0)
        {
            yield return null;
        }

        SetScale(1.0f);
    }

    public void SetScale(float scale = 0.0f)
    {
        Time.timeScale = scale;
        float newFixedTime = scale <= 0.0f ? initFixedDelta : initFixedDelta * Mathf.Max(scale, 0.05f);
        Time.fixedDeltaTime = newFixedTime;
    }
}
