using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthTexturing : MonoBehaviour 
{
    void OnEnable()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.depthTextureMode = DepthTextureMode.Depth;
        }
    }

    void OnDisable()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.depthTextureMode = DepthTextureMode.None;
        }
    }
}
