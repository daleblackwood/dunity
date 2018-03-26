using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pushable : MonoBehaviour 
{
    public float resistTime = 0.2f;
    public float pushTime = 0.5f;
    public float pushSpeed = 0.5f;
    Rigidbody body = null;
    Rigidbody pusher = null;
    Vector3 pushDir = Vector3.zero;
    float connectedTime = 0.0f;
    float initMaxAngularVelocity = 7.0f;
    float timeToRemove = 0.0f;
    float pushElapsed = 0.0f;
    RigidbodyConstraints initConstraints;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        initMaxAngularVelocity = body.maxAngularVelocity;
        initConstraints = body.constraints;
        body.maxAngularVelocity = 0.1f;
    }

    void OnDisable()
    {
        body.maxAngularVelocity = initMaxAngularVelocity;
        body.constraints = initConstraints;
    }

    void FixedUpdate()
    {
        pushElapsed += Time.fixedDeltaTime;

        if (pusher != null)
        {
            if (pushElapsed > 0.0f) 
            {
                pushElapsed = 0.0f;
                connectedTime = Time.time + resistTime;
            }
        }

        if (pusher != null && pusher.gameObject.activeInHierarchy == false)
        {
            timeToRemove = 0.1f;
        }

        RigidbodyConstraints newContraints = initConstraints;

        if (pushElapsed >= 0.0f && pushElapsed < pushTime)
        {
            var force = pushDir * pushSpeed;
            force.y = body.velocity.y;
            body.velocity = force;
            newContraints = initConstraints;
        }
        else if (body.velocity.sqrMagnitude < 0.1f)
        {
            newContraints = initConstraints | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }

        if (body.constraints != newContraints)
        {
            body.constraints = newContraints;
        }

        if (timeToRemove > 0.0f)
        {
            timeToRemove -= Time.fixedDeltaTime;
            if (timeToRemove <= 0.0f)
            {
                pusher = null;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null)
            return;

        if (collision.rigidbody == pusher)
        {
            timeToRemove = 0.0f;
            return;
        }

        if (collision.rigidbody.GetComponent<Actor>() == null)
            return;

        pusher = collision.rigidbody;
        pushElapsed = -resistTime;
        timeToRemove = 0.0f;

        pushDir = transform.position - pusher.transform.position;
        pushDir.y = 0.0f;
        pushDir.Normalize();
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == pusher)
        {
            timeToRemove = 0.1f;
        }
    }
}
