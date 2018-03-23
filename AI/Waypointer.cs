using UnityEngine;
using System.Collections.Generic;

public class Waypointer
{   
    public bool finished { get; private set; }
    public bool succeeded { get; private set; }

    public float nearDistance = 2.0f;
    public bool canFail = true;

    public System.Action onFinish = null;
    public System.Action<Transform> onReachNode = null;
    public System.Action<Transform> onSelectNode = null;
    
    protected Transform host = null;
    protected Transform target = null;
    protected Transform[] nodes = null;

    protected Transform selected = null;
    protected Transform rejected = null;

    public Waypointer() {}
    
    public void Init(Transform host, Transform target, Transform[] wayNodes)
    {
        this.host = host;
        this.target = target;

        var nodeList = new List<Transform>();
        foreach (var node in wayNodes)
        {
            if (node == target)
                continue;

            nodeList.Add(node);
        }
        nodeList.Add(target);
        this.nodes = nodeList.ToArray();

        selected = null;
        rejected = null;
        finished = false;
        succeeded = false;

        OnInit();
    }

    protected virtual void OnInit() {}

    protected virtual void FindNextWaypoint()
    {
        float closestSq = float.PositiveInfinity;

        if (nodes == null)
            return;

        foreach (var node in nodes)
        {
            if (node == null)
                continue;

            var hostDif = node.position - host.position;
            if (hostDif.sqrMagnitude < nearDistance * nearDistance)
                continue;

            var targetDif = node.position - target.position;
            var distSq = targetDif.sqrMagnitude;
            if (distSq > closestSq)
                continue;

            if (CanReachNode(node) == false)
                continue;

            rejected = selected;
            selected = node;
            closestSq = distSq;
        }

        SelectNode(selected);
    }

    public virtual bool CanReachNode(Transform node)
    {
        return CanReachPosition(node.position);
    }

    public virtual bool CanReachPosition(Vector3 position)
    {
        var from = host.position;
        from.y += 0.5f;

        var to = position;
        to.y += 0.5f;

        var dif = to - from;
        var dir = dif.normalized;
        var dist = dif.magnitude;

        RaycastHit hitInfo;
        if (Physics.Raycast(from, dir, out hitInfo, dist, 0xFF, QueryTriggerInteraction.Ignore))
        {
            if ((hitInfo.transform.position - position).sqrMagnitude > 0.1f)
            {
                Debug.DrawLine(from, hitInfo.point, Color.red);
                return false;
            }
        }
        else
        {
            Debug.DrawLine(from, to, Color.green);
        }

        return true;
    }

    public virtual void Update(ref Vector3 targetPos)
    {
        if (host == null || target == null)
        {
            OnFinish(false);
            return;
        }

        if (finished == false && selected != null)
        {
            var dif = selected.position - host.position;
            dif.y = 0.0f;
            if (dif.sqrMagnitude < nearDistance * nearDistance)
            {
                ReachNode(selected);
                if (selected == target)
                {
                    OnFinish(true);
                    return;
                }
                else
                {
                    selected = null;
                }
            }
        }

        if (finished)
        {
            return;
        }

        if (selected != null)
        {
            if (CanReachNode(selected) == false)
            {
                selected = null;
            }
        }

        if (selected == null)
        {
            FindNextWaypoint();
        }

        if (selected != null)
        {
            if (Utils.GetClosestOf(selected, host, target.position) == target)
            {
                targetPos = target.position;
            }
            else
            {
                targetPos = selected.position;
            }
            return;
        }

        if (canFail) 
        {
            OnFinish(false);
        }
    }

    public int GetNodeIndex(Transform node)
    {
        for (int i=0; i<nodes.Length; i++)
        {
            if (nodes[i] == node)
                return i;
        }
        return -1;
    }

    protected virtual void ReachNode(Transform node)
    {
        if (onReachNode != null)
        {
            onReachNode(node);
        }
    }

    protected virtual void SelectNode(Transform node)
    {
        if (onSelectNode != null)
        {
            onSelectNode(node);
        }
    }

    protected virtual void OnFinish(bool success)
    {
        finished = true;
        succeeded = success;
        selected = null;
        rejected = null;
        if (onFinish != null)
        {
            onFinish();
        }
    }

    public virtual void DrawDebug()
    {
        if (target == null || host == null)
            return;
        
        Debug.DrawLine(target.position, host.position, Color.white);

        foreach (var wp in nodes)
        {
            DebugTools.DrawSphere(wp.position, Color.white, 0.3f);
        }

        if (finished)
            return;

        if (selected != null)
        {
            Debug.DrawLine(host.position, selected.position, Color.green);
        }
        if (rejected != null)
        {
            Debug.DrawLine(host.position, rejected.position, Color.grey);
        }
        if (target != null)
        {
            DebugTools.DrawCircle(target.position + Vector3.up, Color.red);
        }
    }

    
}