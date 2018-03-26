using UnityEngine;
using System.Collections;

public class ItemSpin : MonoBehaviour
{
    public Vector3 rotate = Vector3.up;

    void Update()
    {
        transform.rotation *= Quaternion.Euler(Time.unscaledDeltaTime * rotate * 90.0f);
    }
}
