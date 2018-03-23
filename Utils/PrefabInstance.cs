using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using System.Collections.Generic;

// Derived from http://framebunker.com/blog/poor-mans-nested-prefabs/

[ExecuteInEditMode]
public class PrefabInstance : MonoBehaviour
{
    public GameObject prefab;
    public GameObject instance { get; private set; } 

    void OnEnable()
    {
        if (Application.isPlaying)
        {
            Place();
        }
        #if UNITY_EDITOR
        else if (enabled)
        {
            Rebuild();
        }
        #endif
    }

    public void Place()
    {
        if (instance != null)
        {
            instance.SetActive(true);
            return;
        }

        if (Application.isPlaying)
        {
            foreach (Transform child in transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }

            instance = GameObject.Instantiate(prefab);
            instance.transform.SetParent(transform, true);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localScale = prefab.transform.localScale * transform.localScale.x;
            instance.transform.localRotation = prefab.transform.localRotation * transform.localRotation;
        }
    }

    #if UNITY_EDITOR    
    // Struct of all components. Used for edit-time visualization and gizmo drawing
    public struct PrefabAvatar 
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public List<Material> materials;
    }

    [System.NonSerializedAttribute] public List<PrefabAvatar> avatars = new List<PrefabAvatar> ();

    void OnValidate() 
    {
        if (enabled)
        {
            Rebuild();
        }
    }

    void Rebuild()
    {
        avatars.Clear();
        BuildParts(prefab, Matrix4x4.identity);
    }

    void BuildParts(GameObject source, Matrix4x4 inMatrix) 
    {
        if (!source)
            return;

        Matrix4x4 baseMatrix = inMatrix * Matrix4x4.TRS(-source.transform.position, Quaternion.identity, Vector3.one);

        var mrs = source.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var mr in mrs)
        {
            var mf = mr.GetComponent<MeshFilter>();
            if (mf == null)
                continue;

            avatars.Add(new PrefabAvatar() {
                mesh = mf.sharedMesh,
                matrix = baseMatrix * mr.transform.localToWorldMatrix,
                materials = new List<Material>(mr.sharedMaterials)
            });
        }

        var smrs = source.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var smr in smrs)
        {
            var matrix = baseMatrix;

            /*
            var root = smr.rootBone;
            if (root != null)
            {
                var armature = root.parent;
                matrix *= Matrix4x4.TRS(armature.localPosition, armature.localRotation, armature.localScale);
                matrix *= smr.localToWorldMatrix;
            }
            else
            */
            {
                matrix = baseMatrix * smr.localToWorldMatrix;
            }

            avatars.Add(new PrefabAvatar()
            {
                mesh = smr.sharedMesh,
                matrix = matrix,
                materials = new List<Material>(smr.sharedMaterials)
            });
        }

        foreach (PrefabInstance pi in source.GetComponentsInChildren(typeof(PrefabInstance), true))
        {
            if (pi.enabled && pi.gameObject.activeSelf)
            {
                BuildParts(pi.prefab, baseMatrix * pi.transform.localToWorldMatrix);
            }
        }       
    }

    // Editor-time-only update: Draw the meshes so we can see the objects in the scene view
    void Update () 
    {
        if (EditorApplication.isPlaying)
            return;
        
        Matrix4x4 matrix = transform.localToWorldMatrix;
        foreach (PrefabAvatar t in avatars)
        {
            for (int i = 0; i < t.materials.Count; i++)
            {
                if (t.mesh == null || i >= t.mesh.subMeshCount)
                    continue;
                Graphics.DrawMesh(t.mesh, matrix * t.matrix, t.materials[i], gameObject.layer, null, i);
            }
        }
    }

    // Picking logic: Since we don't have gizmos.drawmesh, draw a bounding cube around each thingy
    void OnDrawGizmos () { DrawGizmos (new Color (0,0,0,0)); }
    void OnDrawGizmosSelected () { DrawGizmos (new Color (1,1,1,0.5f)); }
    void DrawGizmos (Color col) {
        if (EditorApplication.isPlaying)
            return;
        
        Gizmos.color = col;
        Matrix4x4 mat = transform.localToWorldMatrix;
        foreach (PrefabAvatar t in avatars)
        {
            if (t.mesh == null)
                continue;
            
            Gizmos.matrix = mat * t.matrix;
            //Gizmos.DrawCube(t.mesh.bounds.center, t.mesh.bounds.size);
            Gizmos.DrawWireCube(t.mesh.bounds.center, t.mesh.bounds.size);
        }       
    }

    new public T GetComponent<T>()
    {
        if (instance == null)
        {
            Place();
        }
        return instance.GetComponent<T>();
    }

    new public T GetComponentInChildren<T>(bool includeInactive = true)
    {
        if (instance == null)
        {
            Place();
        }
        return instance.GetComponentInChildren<T>(includeInactive);
    }

    /*
    // Baking stuff: Copy in all the referenced objects into the scene on play or build
    [PostProcessScene(-2)]
    public static void OnPostprocessScene() 
    { 
        foreach (PrefabInstance pi in UnityEngine.Object.FindObjectsOfType(typeof(PrefabInstance)))
        {
            BakeInstance(pi);
        }
    }

    public static void BakeInstance (PrefabInstance pi) 
    {
        if (!pi.prefab || !pi.enabled)
        {
            return;
        }

        pi.enabled = false;
        GameObject go = PrefabUtility.InstantiatePrefab(pi.prefab) as GameObject;
        Quaternion rot = go.transform.localRotation;
        Vector3 scale = go.transform.localScale;
        go.transform.parent = pi.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = scale;
        go.transform.localRotation = rot;
        pi.prefab = null;
        foreach (PrefabInstance childPi in go.GetComponentsInChildren<PrefabInstance>())
        {
            BakeInstance(childPi);
        }
    }
    */

    #endif
}