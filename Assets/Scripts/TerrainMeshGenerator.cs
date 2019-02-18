using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MapGenerator))]
public class TerrainMeshGenerator : MonoBehaviour
{

    
    public Gradient gradient;

    private int xSize = 20;
    private int zSize = 20;

    private Vector3[] vertices; //world point of vertices
    private int[] triangles; //Index of vertices of each triangles.
    private Color[] colors;
    private Mesh mesh;
    private MapGenerator mapGen;

    public Vector3[] Vertices
    {
        get => vertices;
    }
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mapGen = GetComponent<MapGenerator>();

        mapGen.OnMapChange += GeneratePlane;

        GeneratePlane();
    }
    
    void Update()
    {
        UpdateMesh();
    }
    
    

    private void GeneratePlane()
    {
        if (mapGen.NoiseMap == null) mapGen.GenerateNoiseMap();

        xSize = mapGen.Width;
        zSize = mapGen.Height;

        //Get all vertices
        vertices = new Vector3[(xSize + 1) * (zSize+ 1)];

        Vector3 point;
        //maxHeight = Mathf.NegativeInfinity;
        //minHeight = Mathf.Infinity;
        int i = 0;

        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                point = new Vector3(x, mapGen.HeightValue(z, x), z);
                vertices[i++] = point;
            }
        }

        //Generatal all triangles
        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int triOffset = 0;

        //float waitTime = 0.0001f;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                //Make a quad ---

                //First Triangle
                triangles[triOffset] = vert;
                triangles[triOffset + 1] = vert + xSize + 1;
                triangles[triOffset + 2] = vert + 1;
                //yield return new WaitForSeconds(waitTime); 

                //Second Triangle
                triangles[triOffset + 3] = vert + 1;
                triangles[triOffset + 4] = vert + xSize + 1;
                triangles[triOffset + 5] = vert + xSize + 2;
                //yield return new WaitForSeconds(waitTime); 

                vert++;
                triOffset += 6;

            }

            vert++;

        }

        //Get colors gradient
        colors = new Color[vertices.Length];
        i = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {

                colors[i] = mapGen.EvaluateGradientPoint(i);
                //colors[i] = mapGen.Gradient.Evaluate(Mathf.InverseLerp(mapGen.MinHeight, mapGen.MaxHeight, vertices[i].y));
                i++;
            }
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

    /*
    private float SampleOctaveNoise(float x, float y)
    {
        return SampleOctaveNoise(x, y, octaves, persistence, lacunarity, offset, new Vector2(scale, scale));
    } */
    private float SampleOctaveNoise(float x, float y, int octaves, float persistence, float lacunarity, Vector2 offset, Vector2 scale)
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

    private void OnDrawGizmos()
    {
        if (vertices == null || !Application.isPlaying)
        {
            return;
        }

        return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + vertices[i], 0.05f);
        }
    }
}
