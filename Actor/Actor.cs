using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : GameBehaviour 
{
    new public Rigidbody rigidbody { get; private set; }

    public Mover mover { get; private set; }
    public Mover lastMover { get; private set; }
    public string moverName = null;
    Mover firstMover = null;
    Dictionary<int, Mover> movers = new Dictionary<int, Mover>();

    protected override void OnInit()
    {
        rigidbody = GetComponent<Rigidbody>();
        AttachMovers();
    }

    protected override void OnReady()
    {
        PickMover();
        if (mover != null)
        {
            mover.enabled = false;
            mover.enabled = true;
        }
    }

    protected void AttachMovers()
    {
        var attachedMovers = GetComponentsInChildren<Mover>();
        foreach (var m in attachedMovers)
        {
            int hash = m.GetType().GetHashCode();
            movers[hash] = m;
        }

        if (mover == null && attachedMovers.Length > 0)
        {
            firstMover = attachedMovers[0];
            SetMover(firstMover);
        }
    }

    public virtual void PickMover()
    {
        SetMover(firstMover);
    }

    public bool HasMover<T>() where T : Mover
    {
        int hash = typeof(T).GetHashCode();
        return movers.ContainsKey(hash);
    }

    public bool IsMover<T>() where T : Mover
    {
        if (mover == null)
            return false;

        return mover.GetType() == typeof(T);
    }

    public T GetMover<T>() where T : Mover
    {
        int hash = typeof(T).GetHashCode();
        if (movers.ContainsKey(hash) == false)
            return null;

        return (T)movers[hash];
    }

    public bool SetLastMover()
    {
        return SetMover(lastMover);
    }

    public T SetMover<T>() where T : Mover
    {
        if (mover != null && mover.GetType() == typeof(T))
            return (T)mover;

        int hash = typeof(T).GetHashCode();
        if (movers.ContainsKey(hash) == false)
        {
                Debug.LogWarning(name + " doesn't have mover " + typeof(T).Name);
            return null;
        }

        var newMover = SetMover(movers[hash]);
        return (T)newMover;
    }

    public Mover SetMover(Mover newMover)
    {
        if (mover != null && mover.immutable)
            return null;

        if (mover == newMover)
            return mover;

        Mover found = null;
        if (newMover != null)
        {
            foreach (var pair in movers)
            {
                var m = pair.Value;

                if (m == newMover)
                {
                    found = m;
                }
                else
                {
                    m.enabled = false;
                }
            }

            if (found == null)
            {
                Debug.LogWarning(name + " can't set mover " + newMover.GetType().Name);
                return null;
            }
        }

        lastMover = mover;
        mover = found;
        if (mover != null)
        {
            if (lastMover != null)
            {
                mover.UpdateActions(lastMover.actions);
            }
            mover.enabled = true;
        }
        moverName = mover == null ? "None" : mover.GetType().Name;

        if (mover != null)
        {
            mover.enabled = false;
            mover.enabled = true;
            OnMoverChange();
        }

        return mover;
    }

    protected virtual void OnMoverChange(){}

    public bool TakeHit(Actor other = null, int hits = 1)
    {
        /*
        if (base.TakeHit(other, hits) == false)
            return false;
        
        if (other != null)
        {
            SetTarget(other);
            SetMover<HitMover>();
        }
        */

        Debug.Log("Hit " + name);

        return true;
    }

    public void TurnTowards(Transform other, float turnSpeed = 180.0f)
    {
        var dif = other.position - transform.position;
        dif.y = 0.0f;

        var rotTo = Quaternion.LookRotation(dif);
        var rotate = Time.fixedDeltaTime * turnSpeed * 100.0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTo, rotate);
    }

    public void TurnTowardsDirection(Vector3 dir, float turnSpeed = 180.0f)
    {
        var rotTo = Quaternion.LookRotation(dir.normalized);
        var rotate = Time.fixedDeltaTime * turnSpeed * 100.0f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotTo, rotate);
    }

}
