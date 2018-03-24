using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCam : MonoBehaviour 
{
    static public BaseCam main { get; private set; }

    public Transform[] targets;
    public Collider targetConstrainer;
    public float fieldOfView = 30.0f;

    protected Bounds targetBounds;
    protected Vector3 targetPos;
    protected Camera cam = null;

    float shakePower = 0.0f;
    float joltPower = 0.0f;
    Quaternion shakeRot = Quaternion.identity;
    protected Dictionary<Transform, ActorMover> movers = new Dictionary<Transform, ActorMover>();

    protected virtual void OnEnable()
    {
        main = this;
        cam = GetComponent<Camera>();
        movers.Clear();
    }

    protected void LateUpdate()
    {
        if (Game.isReady == false)
            return;
        
        if (targets == null || targets.Length < 1)
        {
            var targetList = new List<Transform>();
            var players = GameObject.FindObjectsOfType<Player>();
            foreach (var player in players)
            {
                targetList.Add(player.actor.mover.transform);
            }
            targets = targetList.ToArray();
            movers.Clear();
        }

        if (targets.Length < 1)
            return;

        if (movers == null || movers.Count != targets.Length)
        {
            foreach (var target in targets)
            {
                var mover = target.GetComponentInChildren<ActorMover>();
                movers[target] = mover;
            }
        }

        UpdateTargetPos();
        UpdateCamera();

        if (shakePower > 0.001f)
        {
            UpdateShake();
        }

        cam.fieldOfView = fieldOfView;
    }

    protected virtual void UpdateTargetPos()
    {
        targetBounds = new Bounds();
        targetPos = Vector3.zero;
        foreach (var target in targets)
        {
            if (target == null)
                continue;

            var pos = target.position;
            if (movers.ContainsKey(target))
            {
                var mover = movers[target];
                if (mover != null)
                {
                    pos.y = mover.groundY;
                }
            }
            targetBounds.Encapsulate(pos);
            targetPos += pos;
        }
        targetPos /= Mathf.Max(1.0f, (float)(targets.Length));
        targetPos = Constrain(targetPos);
    }

    public Vector3 Constrain(Vector3 position)
    {
        if (targetConstrainer != null)
        {
            position = targetConstrainer.ClosestPoint(targetPos);
        }
        return position;
    }

    protected abstract void UpdateCamera();

    protected virtual void UpdateShake()
    {
        float phase = Time.time* 20.0f;
        var angles = new Vector3(Mathf.Sin(phase), 0.0f, Mathf.Cos(phase)) * shakePower;
        var shakeRotTo = Quaternion.Euler(angles);
        shakeRot = Quaternion.Lerp(shakeRot, shakeRotTo, MathUtils.DeltaEase(0.5f));
        transform.localRotation *= shakeRot;
        shakePower = Mathf.Lerp(shakePower, 0.0f, MathUtils.DeltaEase(0.1f));

        float jolt = joltPower * 0.6f;
        transform.localPosition += new Vector3(Random.Range(-jolt, jolt), Random.Range(-jolt, jolt), Random.Range(-jolt, jolt));
        joltPower = Mathf.Lerp(joltPower, 0.0f, MathUtils.DeltaEase(0.5f));
    }

    public virtual void Shake(float power = 1.0f)
    {
        shakePower = Mathf.Max(shakePower, power * 15.0f);
    }

    public virtual void Jolt(float power = 1.0f)
    {
        joltPower = Mathf.Max(joltPower, power);
    }
}
