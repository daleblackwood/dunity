using UnityEngine;
using System.Collections;
using System.Linq;

public class TerrainTools : MonoBehaviour 
{
    [System.Serializable]
    public class SplatMapProps
    {
        [Range(0.0f, 0.1f)] public float heightFactor = 0.01f;
        [Range(0.0f, 1.0f)] public float cliffFactor = 0.0f;
    }

    [System.Serializable]
    public class TextureProps
    {
        public Texture2D[] textures;
        public float[] sizes;
    }

    public SplatMapProps splatMapProps;
    public TextureProps textureProps;
    public Material baseMaterial;

    // adapted from https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/
    public void GenerateSplatMap() 
    {
        // Get the attached terrain component
        Terrain terrain = GetComponent<Terrain>();

        var props = splatMapProps;

        // Get a reference to the terrain data
        TerrainData terrainData = terrain.terrainData;

        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)terrainData.alphamapHeight;
                float x_01 = (float)x/(float)terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                int tx = Mathf.RoundToInt(y_01 * terrainData.heightmapHeight);
                int ty = Mathf.RoundToInt(x_01 * terrainData.heightmapWidth);
                float height = terrainData.GetHeight(tx, ty);

                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01, x_01);

                int layers = terrainData.alphamapLayers;

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[layers];

                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT

                float heightScaled = terrainData.heightmapHeight * props.heightFactor;

				float cliffFactor = Mathf.Clamp(1.2f - props.cliffFactor, 0.1f, 1.0f);
                float modLowAltitude = Mathf.Clamp01(heightScaled - height);
                float modHighAltitude = height * Mathf.Clamp01(normal.z);
                float modSteepness = Mathf.Clamp01(steepness * steepness / (terrainData.heightmapHeight * cliffFactor));

				float modSteepThreshhold = 0.5f;

				modSteepness = Mathf.Pow(modSteepness, 8.0f * cliffFactor) * 10.0f;

                if (layers > 2)//&& modSteepness >= modSteepThreshhold)
                {
					splatWeights[2] = modSteepness;
                }
                
				//if (modSteepness < 0.5f)
                {
                    // Texture[0] has constant influence
                    splatWeights[0] = modLowAltitude;

                    if (layers > 1)
                    {
                        // Texture[1] is stronger at lower altitudes
                        splatWeights[1] = 1.0f - modLowAltitude;
                    }
                
                    /*
                if (layers > 2)
                {
                    // Texture[2] stronger on flatter terrain
                    // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                    // Subtract result from 1.0 to give greater weighting to flat surfaces
                    splatWeights[2] = 1.0f - modSteepness;
                }
                */

                    if (layers > 3)
                    {
                        // Texture[3] increases with height but only on surfaces facing positive Z axis 
                        splatWeights[3] = modHighAltitude;
                    }
                }

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for(int i = 0; i<terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    public void ApplyTextures()
    {
        // Get the attached terrain component
        Terrain terrain = GetComponent<Terrain>();

        var props = textureProps;

        // Get a reference to the terrain data
        TerrainData terrainData = terrain.terrainData;
        float size = 1.0f;

        SplatPrototype[] textures = new SplatPrototype[props.textures.Length];
        for (int i = 0; i < props.textures.Length; i++)
        {
            textures[i] = new SplatPrototype();
            textures[i].texture = props.textures[i];

            if (i < props.sizes.Length)
            {
                size = props.sizes[i];
            }
            textures[i].tileSize = Vector2.one * size;
        }

        if (baseMaterial != null)
        {
            terrain.materialType = Terrain.MaterialType.Custom;
            terrain.materialTemplate = baseMaterial;
        }
        else
        {
            terrain.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
        }

        terrainData.SetDetailResolution(2048, 16);

        terrainData.splatPrototypes = textures;
        GenerateSplatMap();
    }
}