using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCam : MonoBehaviour 
{
    static public BaseCam main { get; private set; }

    public Transform[] targets;
    public Transform camBoxParent;
    public float camboxPadding = 0.0f;
    public bool camboxEnforced = false;
    public float fieldOfView = 30.0f;
    
    public Vector3 worldViewport { get; private set; }
    protected Bounds targetBounds;
    protected Vector3 targetPos;
    protected Camera cam = null;

    Bounds[] camBounds = null;
    Bounds[] camAvoids = null;

    float shakePower = 0.0f;
    float joltPower = 0.0f;
    Quaternion shakeRot = Quaternion.identity;
    protected Dictionary<Transform, ActorMover> movers = new Dictionary<Transform, ActorMover>();

    BaseCam[] otherCams = null;
    Bounds appliedBounds;

    protected virtual void OnEnable()
    {
        if (main == null)
        {
            main = this;
        }
        cam = GetComponent<Camera>();
        otherCams = GetComponents<BaseCam>();
        movers.Clear();

        if (camBoxParent != null)
        {
            var boundsList = new List<Bounds>();
            var avoidsList = new List<Bounds>();
            for (int i=0; i<camBoxParent.childCount; i++)
            {
                var cambox = camBoxParent.GetChild(i);
                var size = cambox.lossyScale;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                size.z = Mathf.Abs(size.z);
                var bounds = new Bounds(cambox.position, size);

                if (cambox.CompareTag("CamAvoid"))
                {
                    avoidsList.Add(bounds);
                }
                else
                {
                    boundsList.Add(bounds);
                }
                cambox.gameObject.SetActive(false);
            }
            camBounds = boundsList.ToArray();
            camAvoids = avoidsList.ToArray();
        }
    }

    protected void LateUpdate()
    {
        if (Game.isReady == false)
            return;

        float yFactor = (float)Screen.height / (float)Screen.width;
        yFactor *= 1.5f;
        worldViewport = new Vector3(1.0f, 0, yFactor) * camboxPadding * 2.0f;
        
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

        if (shakePower > 0.001f || joltPower > 0.001f)
        {
            UpdateShake();
        }

        cam.fieldOfView = fieldOfView;
    }

    protected virtual void UpdateTargetPos()
    {
        targetPos = GetTargetPos();
    }

    public Vector3 GetTargetPos()
    {
        targetBounds = new Bounds();
        var result = Vector3.zero;
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
            result += pos;
        }
        result /= Mathf.Max(1.0f, (float)(targets.Length));
        result = Constrain(result);
        return result;
    }

    public Vector3 Constrain(Vector3 position)
    {
        if (camBounds != null && camBounds.Length > 0)
        {
            var bounds = new Bounds();
            var isSet = false;
            var inAvoid = false;
            var closestWithin = false;
            float closest2 = float.PositiveInfinity;
            float closestCenter2 = float.PositiveInfinity;
            foreach (var cb in camBounds)
            {
                var p = cb.ClosestPoint(position);
                var dif = position - p;
                dif.y = 0.0f;
                var dist2 = dif.sqrMagnitude;
                var center2 = (position - cb.center).sqrMagnitude;
                var thisWithin = dist2 < 0.1f;
                if (closestWithin)
                {
                    if (thisWithin == false)
                        continue;
                        
                    if (thisWithin && center2 > closestCenter2)
                        continue;
                }
                else
                {
                    if (isSet && dist2 > closest2)
                        continue;
                }

                closestCenter2 = center2;
                closest2 = dist2;
                bounds = cb;
                isSet = true;
                closestWithin = thisWithin;
            }

            foreach (var cb in camAvoids)
            {
                if (cb.Contains(position))
                {
                    inAvoid = true;
                    break;
                }
            }

            if (bounds.Contains(position) == false && inAvoid == false && camboxEnforced == false)
            {
                return position;
            }

            if (isSet)
            {
                var min = bounds.min;
                var max = bounds.max;
                var center = bounds.center;

                var pad = camboxPadding;
                min.x = Mathf.Min(min.x + pad, center.x);
                min.z = Mathf.Min(min.z + pad, center.z);
                max.x = Mathf.Max(max.x - pad, center.x);
                max.z = Mathf.Max(max.z - pad, center.z);

                bounds.min = min;
                bounds.max = max;

                //appliedBounds.min = Vector3.Lerp(appliedBounds.min, bounds.min, MathUtils.DeltaEase(0.1f));
                //appliedBounds.max = Vector3.Lerp(appliedBounds.max, bounds.max, MathUtils.DeltaEase(0.1f));
                appliedBounds = bounds;
                position = appliedBounds.ClosestPoint(position);
            }
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

    public virtual void SkipTo()
    {
        targetPos = GetTargetPos();
    }

    public T SwitchTo<T>() where T : BaseCam
    {
        T result = null;
        foreach (var cam in otherCams)
        {
            bool use = cam is T;
            if (use)
            {
                result = (T)cam;
            }
            cam.enabled = cam is T;
        }
        return result;
    }

    public void SwitchToDefault()
    {
        foreach (var cam in otherCams)
        {
            if (cam == null)
                continue;
            cam.enabled = cam == main;
        }
    }

    void OnDrawGizmosSelected()
    {
        var pos = targetPos;

        var min = pos - worldViewport * 0.5f;
        var max = min + worldViewport;
        DebugTools.DrawCube(min, max, Color.red);

        DebugTools.DrawCube(appliedBounds, Color.blue);

        min = appliedBounds.min - worldViewport * 0.5f;
        max = appliedBounds.max + worldViewport * 0.5f;
        DebugTools.DrawCube(min, max, Color.blue);

    }
}
