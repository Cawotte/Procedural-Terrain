using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer = null;
    [SerializeField] private MapGenerator mapGen = null;
    [Range(0.1f, 1f)]
    [SerializeField] private float scale = 1f;
    

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        mapGen.OnMapChange += UpdateNoiseTexture;
        UpdateNoiseTexture();
    }

    private void OnValidate()
    {
        if (Application.isPlaying && meshFilter != null)
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
        /*
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = mapGen.Gradient.Evaluate(mapGen.NormalizedNoise(mapGen.NoiseMap.Length - i - 1));

            //colors[i] = mapGen.EvaluatePoint()
        }*/

        textureNoise.SetPixels(colors);
        textureNoise.Apply();

        textureRenderer.material.mainTexture = textureNoise;
        //meshFilter.mesh.RecalculateBounds();
        ScaleTextureToScreen(scale);

    }

    /*
    private IEnumerator UpdateNoiseTexture()
    {
        Texture2D textureNoise = new Texture2D(mapGen.Width, mapGen.Height);

        Color[] colors = new Color[mapGen.NoiseMap.Length];
        for (int y = 0, i = 0; y < mapGen.Height; y++)
        {
            for (int x = 0; x < mapGen.Width; x++)
            {
                //Colors start from top right corner, so we need to fill it from the end.
                int index = colors.Length - i - 1;
                colors[i] = mapGen.EvaluateGradient(x, y);
                
                textureNoise.SetPixel(x, y, colors[i]);
                textureNoise.Apply();
                textureRenderer.material.mainTexture = textureNoise;
                //meshFilter.mesh.RecalculateBounds();
                ScaleTextureToScreen(scale);
                yield return new WaitForSeconds(0.001f);
                i++;

                //
            }
        }
        /*
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = mapGen.Gradient.Evaluate(mapGen.NormalizedNoise(mapGen.NoiseMap.Length - i - 1));

            //colors[i] = mapGen.EvaluatePoint()
        }

        textureNoise.SetPixels(colors);
        textureNoise.Apply();

        textureRenderer.material.mainTexture = textureNoise;
        //meshFilter.mesh.RecalculateBounds();
        ScaleTextureToScreen(scale);

    } */

    private void ScaleTextureToScreen(float scale)
    {
        float ratio = (float)mapGen.Height / (float)mapGen.Width;

        float xScale = Camera.main.scaledPixelWidth / meshFilter.mesh.bounds.size.x;
        float yScale = Camera.main.scaledPixelHeight / meshFilter.mesh.bounds.size.z;

        //xScale2 = xScale;
        //yScale2 = yScale;
        /*
        xScale2 = meshFilter.mesh.bounds.size.x;
        yScale2 = meshFilter.mesh.bounds.size.y;
        zScale2 = meshFilter.mesh.bounds.size.z; */

        if (xScale < yScale)
        {
            ratio = Mathf.Pow(ratio, -1f);
            textureRenderer.transform.localScale = new Vector3(yScale * scale * ratio, 1f, yScale * scale);
        }
        else
        {
            textureRenderer.transform.localScale = new Vector3(xScale * scale, 1f, xScale * ratio * scale);
        }
    }
    

}
