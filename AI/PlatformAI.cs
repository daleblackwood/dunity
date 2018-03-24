using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAI : BaseAI
{
    public enum State
    {
        GetTarget,
        GetUp,
        GetDown,
        Wait
    }
    public State state = State.GetTarget;
    State prevState = State.Wait;

    public Transform waypointParent = null;
    public Transform[] path = null;
    public int pathI = 0;
    Transform target = null;
    PlatformSensor sensor;
    float stillTime = 0.0f;
    float stateTime = 0.0f;
    float stateGroundY = 0.0f;
    Vector3 stateDir = Vector3.right;
    float waitTime = 0.0f;

    public string info = "";

    public override void OnReady()
    {   
        base.OnReady();
        sensor = new PlatformSensor(actor.mover);
        PickState();
    }

    public override void Reset()
    {
        base.Reset();
        PickState();
    }

    protected virtual bool PickState()
    {
        PickTarget();

        if (target == null)
        {
            return Wait(1.0f);
        }

        var difY = target.position.y - sensor.mover.groundY;
        /*if (Mathf.Abs(difY) > 10.0f)
        {
            path.Clear();
            PickTarget();
        }
        */

        if (difY > 5.0f)
        {
            return SetState(State.GetUp);
        }

        if (difY < -2.0f)
        {
            return SetState(State.GetDown);
        }

        return SetState(State.GetTarget);
    }

    protected virtual void PickTarget()
    {
        target = null;

        if (path == null || pathI >= path.Length)
        {
            path = GetPath();
            pathI = 0;
            if (path == null || pathI >= path.Length)
                return;
        }

        if (path.Length > 0)
        {
            var node = path[pathI];
            while ((node.position - transform.position).sqrMagnitude < 4.0f)
            {
                pathI++;
                if (pathI >= path.Length)
                {
                    path = null;
                    break;
                }

                node = path[pathI];
            }

            target = node;
            return;
        }
    }

    protected virtual Transform[] GetPath()
    {
        var list = new List<Transform>();
        if (waypointParent != null)
        {
            for (int i=0; i<waypointParent.childCount; i++)
            {
                var child = waypointParent.GetChild(i);
                if (child.gameObject.activeSelf == false)
                    continue;

                list.Add(child);
            }
        }
        return list.ToArray();
    }

    protected Transform GetCheapest(Transform[] targets)
    {
        Transform cheapest = null;
        float cheapestCost = float.PositiveInfinity;

        foreach (var t in targets)
        {
            float cost = GetCost(t);
            if (cost > cheapestCost)
                continue;

            cheapestCost = cost;
            cheapest = t;
        }

        return cheapest;
    }

    protected virtual float GetCost(Transform target)
    {
        var dif = target.position - transform.position;
        if (dif.sqrMagnitude < 4.0f)
            return float.PositiveInfinity;
        
        dif.y *= 3.0f;
        return dif.sqrMagnitude;
    }

    protected override void UpdateActions()
    {
        sensor.Update();
        if (sensor.mover.timeSinceJump > 0.5f)
        {
            actions.primary = false;
        }

        if (sensor.mover.velocity.sqrMagnitude < 0.1f)
        {
            stillTime += Time.deltaTime;
            if (stillTime > 0.5f)
            {
                targetPos = transform.position - transform.forward * 3.0f;
                Wait(0.4f);
            }
        }
        else
        {
            stillTime = 0.0f;
        }

        bool stateActive = UpdateState();

        base.UpdateActions();

        if (stateActive == false)
        {
            PickState();
        }
    }

    void EvaluateLanding()
    {
        if (path != null && pathI < path.Length)
        {
            var node = path[pathI];
            if (Mathf.Abs(node.position.y - transform.position.y) > 4.0f)
            {
                path = null;
            }
        }
    }

    bool UpdateState()
    {
        switch (state) 
        {
            case State.GetUp: return StateGetUp();
            case State.GetDown: return StateGetDown();
            case State.Wait: return StateWait();
        }
        return StateGetTarget();
    }

    bool SetState(State newState)
    {
        state = newState;
        stateTime = 0.0f;
        stateGroundY = sensor != null ? sensor.mover.groundY : transform.position.y;

        if (target != null)
        {
            var dif = target.position - transform.position;
            dif.y = 0.0f;
            stateDir = dif.normalized;
        }
        else
        {
            stateDir = Camera.main.transform.right;
        }

        return true;
    }

    bool Wait(float time)
    {
        waitTime = time;
        SetState(State.Wait);
        return true;
    }

    bool StateGetUp()
    {
        targetPos = transform.position + stateDir * 3.0f;

        if (sensor.mover.CanJump)
        {
            if (sensor.isUnderOneWayPlatform)
            {
                targetPos = transform.position + stateDir * 3.0f;
                actions.primary = true;
                return Wait(0.7f);
            }

            if (sensor.canJumpWall)
            {
                actions.primary = true;
                return Wait(0.5f);
            }

            if (sensor.isNearFall)
            {
                actions.primary = true;
            }
        }

        if (sensor.isWellUnderPlatform)
        {
            if ((transform.position - target.position).sqrMagnitude < 8.0f)
            {
                targetPos = target.position;
                return Wait(0.5f);
            }
        }

        if (Mathf.Abs(stateGroundY - sensor.mover.groundY) > 2.0f)
            return false;

        if (sensor.mover.onWall)
            return false;

        return true;
    }

    bool StateGetDown()
    {
        targetPos = transform.position + stateDir * 3.0f;

        if (sensor.mover.CanJump)
        {
            if (sensor.canJumpWall)
            {
                actions.primary = true;
            }

            if (sensor.isNearFall)
                return Wait(0.5f);
        }

        if (Mathf.Abs(stateGroundY - sensor.mover.groundY) > 3.0f)
            return false;

        return true;
    }

    bool StateGetTarget()
    {
        targetPos = target.position;
        sensor.Update();

        if (sensor.mover.CanJump)
        {
            if (sensor.canJumpWall)
            {
                actions.primary = true;
            }

            if (sensor.isNearFall)
            {
                actions.primary = true;
            }
        }

        if (Mathf.Abs(stateGroundY - sensor.mover.groundY) > 2.0f)
            return false;

        var dif = target.position - transform.position;
        dif.y = 0.0f;
        if (dif.sqrMagnitude < 4.0f)
            return false;
        
        return true;
    }

    bool StateWait()
    {
        waitTime -= Time.unscaledDeltaTime;
        return waitTime > 0.0f;
    }

    void OnDrawGizmos()
    {
        if (target != null)
        {
            DebugTools.DrawSphere(target.position, Color.red);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.9f)
        {
            EvaluateLanding();
        }
    }
}