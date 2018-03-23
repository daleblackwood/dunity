using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour 
{
    Dictionary<string, Material> materials = new Dictionary<string, Material>();

    public static MaterialManager current { get; private set; }

    public Material[] preloaded;

    void Awake()
    {
        current = this;
        materials.Clear();

        foreach (var m in preloaded)
        {
            string key = m.name.ToLower();
            materials[key] = m;
        }
    }

    public Material TextureMaterial(Texture texture, bool useLight, bool clamp)
    {
        string key = texture.name;
        if (useLight)
        {
            key += "Lit";
        }
        if (clamp)
        {
            key += "Clamp";
        }

        if (materials.ContainsKey(key) == false)
        {
            var shaderName = useLight ? "Transparent/Diffuse" : "Unlit/Transparent";
            var material = new Material(Shader.Find(shaderName));
            material.name = key;

            texture.wrapMode = clamp ? TextureWrapMode.Clamp : TextureWrapMode.Repeat;
            material.SetTexture("_MainTex", texture);

            materials[key] = material;
        }

        return materials[key];
    }

    public Material ColorMaterial(Color color)
    {
        string key = "C" + ColorUtility.ToHtmlStringRGBA(color);

        if (materials.ContainsKey(key) == false)
        {
            var shaderName = "Unlit/Color";
            var material = new Material(Shader.Find(shaderName));
            material.color = color;
            material.name = key;
            materials[key] = material;
        }

        return materials[key];
    }

    public Material PreloadMaterial(string name)
    {
        string key = name.ToLower();
        if (materials.ContainsKey(key))
        {
            return materials[key];
        }
        return ColorMaterial(new Color(1, 0, 1));
    }
}
