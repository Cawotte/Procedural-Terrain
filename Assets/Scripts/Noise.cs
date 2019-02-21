using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cawotte.Utils;

public class Noise {

    public static Serialized2DArray<float> GenerateNoiseMap(int width, int height, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 scale, int seed = 1)
    {
        Serialized2DArray<float> noiseMap = new Serialized2DArray<float>(width, height);

        //Use the seed to generate a random offset for each on top of the given one
        Vector2[] octaveOffsets = new Vector2[octaves];
        System.Random rng = new System.Random(seed);
        for (int i = 0; i < octaves; i++)
        {
            //octaveOffsets[i] = new Vector2();
            octaveOffsets[i].x = rng.Next(-10000, 10000) + offset.x;
            octaveOffsets[i].y = rng.Next(-10000, 10000) + offset.y;
        }

        float noiseValue;
        float maxNoise = Mathf.NegativeInfinity;
        float minNoise = Mathf.Infinity;

        //Zooming on texture focus on center
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        for (int i = 0, y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {

                noiseValue = SampleOctaveNoise(x - halfWidth, y - halfHeight, octaves, persistence, lacunarity, octaveOffsets, scale);
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

        Vector2 samplePos = new Vector2((x + offset.x) / scale.x, (y + offset.y) / scale.y);
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(samplePos.x * frequency, samplePos.y * frequency) * amplitude;
            totalAmplitude += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return total / totalAmplitude;

    }

    public static float SampleOctaveNoise(float x, float y, int octaves, float persistence, float lacunarity, Vector2[] offsets, Vector2 scale)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float totalAmplitude = 0f;

        Vector2 pos = new Vector2(x / scale.x, y / scale.y);
        for (int i = 0; i < octaves; i++)
        {
            Vector2 samplePos = new Vector2(pos.x * frequency + offsets[i].x, pos.y * frequency + offsets[i].y);
            total += Mathf.PerlinNoise(samplePos.x, samplePos.y) * amplitude;
            totalAmplitude += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }
        return total / totalAmplitude;

    }

}
