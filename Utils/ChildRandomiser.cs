using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRandomiser : MonoBehaviour 
{
    public Vector3 sizeVariance = Vector3.one * 0.2f;
    [Range(0.2f, 10.0f)] public float sizeMultiplier = 1.0f;

    public void Randomise()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var sizeV = new Vector3(
                1.0f + Random.Range(sizeVariance.x * -0.5f, sizeVariance.x * 0.5f),
                1.0f + Random.Range(sizeVariance.y * -0.5f, sizeVariance.y * 0.5f),
                1.0f + Random.Range(sizeVariance.z * -0.5f, sizeVariance.z * 0.5f)
            );
            sizeV *= sizeMultiplier;
            child.localScale = sizeV;
            child.localEulerAngles = Vector3.up * Random.Range(0.0f, 360.0f);
        }
    }
}
