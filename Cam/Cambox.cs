using UnityEngine;
using System.Collections.Generic;

public class Cambox : GameBehaviour
{
    public static Cambox main { get; private set; }

    const float BOUNDS_GIVE = 1.0f;

    new BoxCollider collider;

    protected override void OnInit()
    {
        main = this;
        collider = Utils.Require<BoxCollider>(gameObject);
        collider.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        collider.size = new Vector3(60.0f, 60.0f, 60.0f);
        collider.center = new Vector3(0, collider.size.y * 0.3f, 0);
    }

    void LateUpdate()
    {
        var pos = transform.position;
        if (BaseCam.main != null)
        {
            pos = BaseCam.main.GetTargetPos();
        }
        transform.position = pos;
    }

    public bool IsInsideBounds(Vector3 position)
    {
        var bounds = collider.bounds;
        bounds.min -= Vector3.one * BOUNDS_GIVE;
        bounds.max += Vector3.one * BOUNDS_GIVE;
        return collider.bounds.Contains(position);
    }

    void OnTriggerEnter(Collider other)
    {
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            GameStage.main.ActivateEntity(entity);
        }
    }
    
    /*
    void OnTriggerExit(Collider other)
    {
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            GameStage.main.DeactivateEntity(entity);
        }
    }
    */
}