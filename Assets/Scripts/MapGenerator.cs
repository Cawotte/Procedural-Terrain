using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [Header("Terrain attributes")]
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
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

    private float minHeight;
    private float maxHeight;
    private float[] noiseMap;


    public Action OnMapChange = null;

    public float[] NoiseMap { get => noiseMap; }
    public float MinHeight { get => minHeight; }
    public float MaxHeight { get => maxHeight; }
    public int Width { get => width; }
    public int Height { get => height; }
    public Gradient Gradient { get => gradient; }

    public float this[int x, int y]
    {
        get
        {
            return NoiseMap[x + (y * width)];
        }
    }

    private void OnValidate()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;

        GenerateNoiseMap();
    }

    public float[] GenerateNoiseMap()
    {
        return GenerateNoiseMap(width, height);
    }

    public float[] GenerateNoiseMap(int width, int height)
    {
        noiseMap = new float[(width + 1) * (height + 1)];

        float noiseValue;
        maxHeight = Mathf.NegativeInfinity;
        minHeight = Mathf.Infinity;

        for (int i = 0, y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                
                noiseValue = SampleOctaveNoise(x, y);
                //noiseValue = Mathf.Lerp(0, 20, noiseValue);
                noiseMap[i++] = noiseValue;

                //Save min and max;
                if (noiseValue > maxHeight) maxHeight = noiseValue;
                else if (noiseValue < minHeight) minHeight = noiseValue;
            }
        }

        OnMapChange?.Invoke();

        return noiseMap;
    }

    public float HeightValue(int x, int y)
    {
        return Mathf.Lerp(0, 20, this[x, y]);
    }

    public float HeightValue(int index)
    {
        return Mathf.Lerp(0, 20, noiseMap[index]);
    }

    public Color EvaluateGradientPoint(int x, int y)
    {
        return Gradient.Evaluate(Mathf.InverseLerp(MinHeight, MaxHeight, noiseMap[y * width + x]));
    }
    public Color EvaluateGradientPoint(int index)
    {
        return Gradient.Evaluate(Mathf.InverseLerp(MinHeight, MaxHeight, noiseMap[index]));
    }

    public Color EvaluateGradient(float value)
    {
        return Gradient.Evaluate(Mathf.InverseLerp(MinHeight, MaxHeight, value));
    }
    public float NormalizedNoise(int index)
    {
        return Mathf.InverseLerp(minHeight, maxHeight, noiseMap[index]);
    }
    private float SampleOctaveNoise(float x, float y)
    {
        return SampleOctaveNoise(x, y, octaves, persistence, lacunarity, offset, new Vector2(scale, scale));
    }

    private static float SampleOctaveNoise(float x, float y, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 scale)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float totalAmplitude = 0f;

        Vector2 samplePos = new Vector2((x + offset.x) * scale.x, (y + offset.y) * scale.y);
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(samplePos.x * frequency, samplePos.y * frequency) * amplitude;
            totalAmplitude += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return total / totalAmplitude;

    }
}
