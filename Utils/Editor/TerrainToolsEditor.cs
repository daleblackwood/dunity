using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainTools))]
public class TerrainToolsEditor : Editor 
{
    SerializedObject serialized;
    TerrainData swapTerrainData = null;

    void OnEnable()
    {
        serialized = new SerializedObject(target);
    }

    override public void OnInspectorGUI()
    {
        var tools = (TerrainTools)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate SplatMap"))
        {
            tools.GenerateSplatMap();
        }

        if (tools.textureProps != null && tools.textureProps.textures != null && tools.textureProps.textures.Length > 0)
        {
            if (GUILayout.Button("Apply Textures"))
            {
                tools.ApplyTextures();
            }
        }

        swapTerrainData = (TerrainData)EditorGUILayout.ObjectField("Swap Terrain", swapTerrainData, typeof(TerrainData), false);
        if (swapTerrainData != null)
        {
            if (GUILayout.Button("Swap Terrain"))
            {
                var collider = tools.GetComponent<TerrainCollider>();
                collider.terrainData = swapTerrainData;

                var terrain = tools.GetComponent<Terrain>();
                terrain.terrainData = swapTerrainData;

                swapTerrainData = null;
            }
        }
    }
}
