using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cawotte.Utils;

public class Noise {

    public static Serialized2DArray<float> GenerateNoiseMap(int width, int height, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 scale)
    {
        Serialized2DArray<float> noiseMap = new Serialized2DArray<float>(width, height);

        float noiseValue;
        float maxNoise = Mathf.NegativeInfinity;
        float minNoise = Mathf.Infinity;

        for (int i = 0, y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {

                noiseValue = SampleOctaveNoise(x, y, octaves, persistence, lacunarity, offset, scale);
                noiseMap[x, y] = noiseValue;

                //Save min and max so they become the 0 and 1 extremas
                if (noiseValue > maxNoise) maxNoise = noiseValue;
                else if (noiseValue < minNoise) minNoise = noiseValue;
            }
        }

        //Use the min/max has new 0 and 1 bounds.
        for (int y = 0; y < noiseMap.Height; y++)
        {
            for (int x = 0; x < noiseMap.Width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoise, maxNoise, noiseMap[x, y]);
            }
        }
        
        return noiseMap;
    }

    public static float SampleOctaveNoise(float x, float y, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 scale)
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
