using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour 
{
    public float reactionTime = 0.5f;
    public System.Action<Collider> onChange = null;

    public Collider collider = null;
    List<Collider> colliders = new List<Collider>();
    public bool isColliding { get; private set; }
    bool changing = false;
    float changeTime = 0.0f;

    void Update()
    {
        var i = colliders.Count;
        while (i-- > 0)
        {
            if (colliders[i] == null || colliders[i].gameObject.activeInHierarchy == false)
            {
                colliders.RemoveAt(i);
            }
        }

        bool nowColliding = colliders.Count > 0;
        if (nowColliding != isColliding)
        {
            isColliding = nowColliding;
            changeTime = reactionTime;
            changing = true;
            collider = colliders.Count == 0 ? null : colliders[0];
        }
        else
        {
            changeTime -= Time.deltaTime;
        }

        if (changing && changeTime <= 0.0f)
        {
            changing = false;
            changeTime = 0.0f;
            if (onChange != null)
            {
                onChange.Invoke(collider);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var collider = collision.collider;
        if (colliders.Contains(collider) == false)
        {
            colliders.Add(collider);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        var collider = collision.collider;
        if (colliders.Contains(collider))
        {
            colliders.Remove(collider);
        }
    }
}
