using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTilter : MonoBehaviour 
{
    public float maxAngle = 15.0f;
    public float speed = 2.0f;

    float prevAngle = 0.0f;
    float tilt = 0.0f;

    void OnDisable()
    {
        transform.localRotation = Quaternion.identity;
    }
	
	void Update() 
    {
        float angle = transform.eulerAngles.y;

        float tiltTo = angle - prevAngle;
        tiltTo *= -5.0f;

        float maxAngle = 20.0f;
        tiltTo = Mathf.Clamp(tiltTo, -maxAngle, maxAngle);

        float speedDt = Time.deltaTime * 100.0f * speed;

        float maxChange = Mathf.Abs(tiltTo) > Mathf.Abs(tilt) ? speedDt : speedDt;
        float change = Mathf.Clamp(tiltTo - tilt, -maxChange, maxChange);
        tilt += change;

        transform.localRotation = Quaternion.AngleAxis(tilt, Vector3.forward);

        prevAngle = angle;
	}
}
