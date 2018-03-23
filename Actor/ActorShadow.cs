using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorShadow : GameBehaviour 
{
    public float height = 3.0f;
    public float size = 1.0f;

    GameObject child;
    Camera cam;
    RenderTexture renderTex;
    Projector projector;
    Material mat;

    protected override void OnInit()
    {
        renderTex = new RenderTexture(32, 32, 0, RenderTextureFormat.Shadowmap);
        renderTex.Create();
        renderTex.wrapMode = TextureWrapMode.Clamp;

        child = new GameObject("ActorShadowChild");
        child.transform.SetParent(transform, false);

        var matShader = Shader.Find("Projector/Cutout");
        mat = new Material(matShader);
        mat.SetTexture("_ShadowTex", renderTex);

        var layerMask = 1 << LayerMask.NameToLayer("Actor");

        cam = child.AddComponent<Camera>();
        cam.orthographic = true;
        cam.targetTexture = renderTex;
        cam.cullingMask = layerMask;
        cam.backgroundColor = Color.white;

        projector = child.AddComponent<Projector>();
        projector.material = mat;
        projector.orthographic = true;
        projector.ignoreLayers = layerMask;
    }

    void Update()
    {
        cam.orthographicSize = size;
        projector.orthographicSize = size * 0.9f;

        var pos = new Vector3(0, height, 0);
        child.transform.localPosition = pos;

        child.transform.localEulerAngles = new Vector3(90, 0, 0);
    }


}
