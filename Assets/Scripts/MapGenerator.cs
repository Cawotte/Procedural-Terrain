using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cawotte.Utils;

public class MapGenerator : MonoBehaviour
{

    [Header("Terrain attributes")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private float minHeight = 0;
    [SerializeField] private float maxHeight = 20;
    [SerializeField] private Gradient gradient;

    [Header("Noise")]
    [Range(1, 10)]
    public int octaves = 4;
    [Range(0f, 5f)]
    public float persistence = 0.5f;
    [Range(0f, 5f)]
    public float lacunarity = 2f;
    [Range(0.01f, 0.3f)]
    public float scale = 0.1f;
    public Vector2 offset = Vector2.zero;

    //private float[] noiseMap;
    private Serialized2DArray<float> noiseMap;
    [SerializeField] private float[] array;

    public Action OnMapChange = null;

    public Serialized2DArray<float> NoiseMap { get => noiseMap; }
    

    public float MinHeight { get => minHeight; }
    public float MaxHeight { get => maxHeight; }
    public int Width { get => width; }
    public int Height { get => height; }
    public Gradient Gradient { get => gradient; }
    

    private void OnValidate()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;

        GenerateNoiseMap();
    }

    public void GenerateNoiseMap()
    {
        GenerateNoiseMap(width, height);
    }

    public void GenerateNoiseMap(int width, int height)
    {
        noiseMap = Noise.GenerateNoiseMap(width, height, octaves, persistence, lacunarity, offset, new Vector2(scale, scale));
        /*
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                noiseMap[x, y] = Mathf.Lerp(minHeight, maxHeight, noiseMap[x, y]);
            }
        }*/

        OnMapChange?.Invoke();

        array = noiseMap.Array;

    }
    

    public float HeightValue(int x, int y)
    {
        return Mathf.Lerp(minHeight, maxHeight, noiseMap[x, y]);
    }

    public Color EvaluateGradient(int x, int y)
    {
        return Gradient.Evaluate(noiseMap[x, y]);
    }
    
}
