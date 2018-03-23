using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointerPath : Waypointer
{
    public int maxIndex { get; private set; }

    public WaypointerPath() : base() {}

    protected override void OnInit()
    {
        maxIndex = 0;
    }

    public override bool CanReachNode(Transform node)
    {
        int nodeIndex = GetNodeIndex(node);
        if (nodeIndex > maxIndex)
            return false;

        var forward = host.position.normalized;
        var right = Vector3.Cross(forward, Vector3.up);
        var projectedPosition = ProjectPointOnLine(host.position, right, node.position);
        return base.CanReachPosition(projectedPosition);
    }

    protected override void ReachNode(Transform node)
    {
        maxIndex = GetNodeIndex(node) + 1;
        base.ReachNode(node);
    }

    Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        var linePointToPoint = point - linePoint;
        var t = Vector3.Dot(linePointToPoint, lineVec);
        return linePoint + lineVec * t;
    }

}
