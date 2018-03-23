using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHead : MonoBehaviour 
{
    Quaternion initRot;
    public Vector3 angles;

    void Awake()
    {
        initRot = transform.localRotation;
    }

    void LateUpdate()
    {
        var rot = initRot;
        rot *= Quaternion.Euler(angles);
        transform.localRotation = rot;
    }
}
