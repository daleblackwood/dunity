using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamOrbit : MonoBehaviour 
{
    public Transform target;
    public float distance = 10.0f;
    public float angleArc = 30.0f;
    public float angle = 20.0f;
    public float dragStrength = 20.0f;
    public Vector3 offset = Vector3.zero;
    public bool orbitLeftMouse = true;

    Vector3 targetPos = Vector3.zero;
    Vector3 move = Vector3.zero;
    float speed = 0.0f;
    Vector3 lastMousePos;
    Quaternion rot = Quaternion.identity;

    void LateUpdate()
    {
        var mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        var mouseDif = mousePos - lastMousePos;
        lastMousePos = mousePos;

        var targetPosTo = target.position;
        targetPosTo += offset;
        var moveTo = targetPosTo - targetPos;
        moveTo *= 0.3f;

        if (moveTo.magnitude > 0.5f)
        {
            speed += Time.deltaTime * 0.05f;
        }
        else
        {
            speed *= 0.9995f;
        }


        //moveTo = Vector3.ClampMagnitude(moveTo, speed);
        targetPos += moveTo;

        Quaternion rotTo = rot;

        if (Input.GetMouseButton(orbitLeftMouse ? 0 : 1))
        {
            bool orbiting = true;

            if (EventSystem.current.currentSelectedGameObject != null)
            {
                orbiting = false;
            }

            if (orbiting)
            {
                angleArc += mouseDif.y * dragStrength * 0.3f;
                angleArc = Mathf.Clamp(angleArc, -120.0f, 120.0f);
                angle += mouseDif.x * dragStrength;
            }
        }

        float inputAffect = Time.deltaTime * 200.0f;
        angle += Input.GetAxis("Horizontal") * inputAffect;
        distance -= Input.GetAxis("Vertical") * inputAffect * 0.1f;

        distance = Mathf.Clamp(distance, 2.0f, 10.0f);

        rotTo = Quaternion.identity;
        rotTo *= Quaternion.AngleAxis(angle, Vector3.up);
        rotTo *= Quaternion.AngleAxis(angleArc + 90.0f, Vector3.right);

        var camOffset = rotTo * Vector3.up * distance;

        var camPos = targetPos + camOffset;

        transform.position = camPos;
        transform.LookAt(targetPos + transform.rotation * offset);
    }
}
