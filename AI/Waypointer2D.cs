using UnityEngine;
using System.Collections;

public class Waypointer2D : Waypointer
{
    public Waypointer2D() 
        :base(){}

    public override bool CanReachNode(Transform node)
    {
        var projectedPosition = ProjectPointOnLine(host.position, target.forward, node.position);
        return base.CanReachNode(node);
    }

    Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point)
    {
        var linePointToPoint = point - linePoint;
        var t = Vector3.Dot(linePointToPoint, lineVec);
        return linePoint + lineVec * t;
    }
}
