using UnityEngine;

[ExecuteInEditMode]
public class WorldCurver : MonoBehaviour
{
    public Transform curveTarget;
    public float curveOffset;
    public float curveScale;
    public float curveStrength = 5.0f;
    public float curveMax = 5.0f;

    int idCurveOrigin;
    int idCurveScale;
    int idCurveStrength;

    void Start()
    {
        idCurveOrigin = Shader.PropertyToID("_CurveOrigin");
        idCurveScale = Shader.PropertyToID("_CurveScale");
        idCurveStrength = Shader.PropertyToID("_CurveStrength");

        if (curveTarget == null)
        {
            curveTarget = Camera.main.transform;
        }
    }


    void Update()
    {
        var curvePos = curveTarget.position;
        curvePos.z += curveOffset;
        Shader.SetGlobalVector(idCurveOrigin, curvePos);
        Shader.SetGlobalFloat(idCurveScale, curveScale);
        Shader.SetGlobalFloat(idCurveStrength, curveStrength);
        Shader.SetGlobalFloat(idCurveStrength, curveMax);
    }

    void OnDisable()
    {
        Shader.SetGlobalVector(idCurveOrigin, Vector3.zero);
        Shader.SetGlobalFloat(idCurveScale, 0);
    }
}
