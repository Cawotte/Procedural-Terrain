using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AspectRatioFitter))]
public class TerrainTextureDisplay : MonoBehaviour
{
    [SerializeField] private RawImage textureRenderer = null;
    [SerializeField] private MapGenerator mapGen = null;

    //used to avoid a warning on aspectRatio change in OnValidatz
    private bool isDirty = false;
    private void Start()
    {

        mapGen.OnMapChange += UpdateNoiseTexture;
        UpdateNoiseTexture();
    }

    private void Update()
    {
        if (isDirty)
        {
            float ratio = (float)mapGen.Width / (float)mapGen.Height;
            GetComponent<AspectRatioFitter>().aspectRatio = ratio;
            isDirty = false;
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateNoiseTexture();
        }
    }

    private void UpdateNoiseTexture()
    {
        Texture2D textureNoise = new Texture2D(mapGen.Width, mapGen.Height);

        Color[] colors = new Color[mapGen.NoiseMap.Length];
        for (int y = 0, i = 0; y < mapGen.Height; y++)
        {
            for (int x = 0; x < mapGen.Width; x++)
            {
                //Colors start from top right corner, so we need to fill it from the end.
                int index = colors.Length - i - 1;
                colors[index] = mapGen.EvaluateGradient(x, y);
                i++;
                //
            }
        }

        textureNoise.SetPixels(colors);
        textureNoise.Apply();

        textureRenderer.texture = textureNoise;

        isDirty = true;
        //float ratio = (float)mapGen.Width / (float)mapGen.Height;

        //GetComponent<AspectRatioFitter>().aspectRatio = ratio;

    }
    
    

}
