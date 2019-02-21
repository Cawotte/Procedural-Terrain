using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainMeshGenerator : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MapGenerator mapGen = null;

    private Vector3[] vertices; //world point of vertices
    private int[] triangles; //Index of vertices of each triangles.
    private Color[] colors;
    private Mesh mesh;
    
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

        int xSize = mapGen.Width;
        int zSize = mapGen.Height;

        //Get all vertices
        vertices = new Vector3[(xSize + 1) * (zSize+ 1)];

        Vector3 point;

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                point = new Vector3(x, mapGen.HeightValue(x, z), z);
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
        for (int z = 0, i = 0 ; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                
                colors[i++] = mapGen.EvaluateGradient(x, z);
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
    private void OnDrawGizmos()
    {
        if (vertices == null || !Application.isPlaying)
        {
            return;
        }


        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + vertices[i], 0.05f);
        }
    } */
}
