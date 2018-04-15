using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrigger : Triggerable 
{
    public Vector3 offset = Vector3.zero;
    public bool inverted = false;
    public float time = 1.0f;
    public AudioClip clip;
    Vector3 localPos = Vector3.zero;

    override protected void Awake()
    {
        localPos = transform.localPosition;
        base.Awake();
    }

    override protected void OnTrigger()
    {
        if (clip != null)
        {
            //SoundPlayer.main.Play(clip.name);
        }
    }

    void Update()
    {
        var altPos = localPos + offset;

        var toPos = isTriggered == inverted ? localPos : altPos;
        var move = toPos - transform.localPosition;

        move = Vector3.ClampMagnitude(move, Time.unscaledDeltaTime / Mathf.Max(0.1f, time) * move.magnitude);

        transform.localPosition += move;
    }
}
