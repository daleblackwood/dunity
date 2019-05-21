using UnityEngine;

public class PlatformSensor
{
    public ActorMover mover { get; private set; }

    public bool canJumpWall { get; private set; }
    public bool isOnWall { get; private set; }
    public bool isUnderPlatform { get; private set; }
    public bool isWellUnderPlatform { get; private set; }
    public bool isUnderOneWayPlatform { get; private set; }
    public bool isNearFall { get; private set; }

    int layerCast = 0;
    int layerOneWay = 0;
    Collider collider;
    Transform transform;
    RaycastHit hitUp;
    RaycastHit hitDown;
    RaycastHit hitFrontLow;
    RaycastHit hitFrontHigh;
    RaycastHit hitUpHigh;

    public PlatformSensor(ActorMover mover)
    {
        this.mover = mover;
        transform = mover.transform;
        collider = mover.GetComponent<Collider>();
    }

    public void Update()
    {
        Cast(out hitUp, transform.forward * 1.0f, Vector3.up, 3.5f);
        isUnderPlatform = hitUp.collider != null;
        isUnderOneWayPlatform = hitUp.collider != null && hitUp.collider.gameObject.layer == Layers.OneWay;

        Cast(out hitUpHigh, transform.forward * 1.0f, Vector3.up, 6.0f);
        isWellUnderPlatform = hitUpHigh.collider != null;

        Cast(out hitFrontLow, Vector3.zero, transform.forward, 2.0f);
        isOnWall = hitFrontLow.collider != null;

        Cast(out hitFrontHigh, Vector3.up * 3.0f, transform.forward, 2.0f);
        canJumpWall = isOnWall && hitFrontHigh.collider == null;

        Cast(out hitDown, transform.forward * 1.0f, Vector3.down, 1.5f);
        isNearFall = hitDown.collider == null;
    }

    bool Cast(out RaycastHit hitInfo, Vector3 offset, Vector3 direction, float distance)
    {
        direction.Normalize();
        var origin = collider.bounds.center + offset;

        if (Physics.Raycast(origin, direction.normalized, out hitInfo, distance, layerCast))
        {
            Debug.DrawLine(origin, hitInfo.point, Color.red);
            return true;
        }

        Debug.DrawLine(origin, origin + direction * distance, Color.white);
        return false;
    }
}