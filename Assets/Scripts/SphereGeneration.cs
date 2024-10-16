using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DelaunatorSharp;

public class SphereGeneration : MonoBehaviour
{
    [Range(3, 50000)]
    [SerializeField] int numberOfPoints = 10;
    [SerializeField] int radius = 100;

    [SerializeField] NoiseSettings noiseSettings = new NoiseSettings();

    SimpleNoiseFilter noiseFilter;

    Mesh sphereMesh;

    [SerializeField] TileObject[] tiles;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject treeBirch;
    [SerializeField] GameObject treeSpruce;

    void Start()
    {

        GenerateSphere();
    }

    private void OnDrawGizmosSelected()
    {
        //GenerateSphere();
    }

    void GenerateSphere()
    {
        noiseFilter = new SimpleNoiseFilter(noiseSettings);
        //Create sphere
        Vector3[] points = FibonacciPoints(numberOfPoints);
        int[] tris = DelaunayTriangles(points);

        //Add noise
        for (int i = 0; i < points.Length; i++)
        {
            float elevation = noiseFilter.Evaluate(points[i]);
            points[i] -= elevation * points[i].normalized;
        }

        //Set up mesh
        sphereMesh = new Mesh();
        sphereMesh.vertices = points;
        sphereMesh.triangles = tris;
        sphereMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = sphereMesh;

        gameObject.AddComponent<MeshCollider>();

        tiles = new TileObject[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            //Gizmos.DrawSphere(points[i], 0.1f);
            //Handles.Label(points[i], i.ToString());
            tiles[i] = Instantiate(tilePrefab, points[i], Quaternion.Euler(Vector3.zero)).GetComponent<TileObject>();

            if (Random.Range(0, 10) > Mathf.Abs(points[i].y) / 10f + 5)
            {
                tiles[i].SpawnItem(treeBirch);
            }
            if (Random.Range(0, 10) < Mathf.Abs(points[i].y) / 10f - 5)
            {
                tiles[i].SpawnItem(treeSpruce);
            }
        }
    }

    Vector3[] FibonacciPoints(int n)
    {
        Fibonator fibonator = new Fibonator(n, radius);
        return fibonator.Points;
    }

    int[] DelaunayTriangles(Vector3[] points)
    {
        IPoint[] projectedPoints = ProjectToFlat(points);

        Delaunator delaunator = new Delaunator(projectedPoints);

        return delaunator.Triangles;
    }

    IPoint[] ProjectToFlat(Vector3[] points)
    {
        IPoint[] projectedPoints = new IPoint[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            float newX = point.x / (radius - point.y);
            float newY = point.z / (radius - point.y);
            projectedPoints[i] = new Point(newX, newY);
        }
        return projectedPoints;
    }

    int[] CalculateTriangles()
    {
        int[] triangles = new int[numberOfPoints * 6];

        int left = 0;
        int center = 0;
        int right = 0;

        for (int i = 0; i < numberOfPoints / 2; i += 1)
        {
            if (i < 3)
            {
                left = 5;
                center = 13;
                right = 8;
            }
            else if (i < 34)
            {
                left = 13;
                center = 21;
                right = 8;
            }
            else if (i < 144)
            {
                left = 34;
                center = 55;
                right = 21;
            }
            else if (i < 1597)
            {
                left = 34;
                center = 89;
                right = 55;
            }
            else if (i < 2584)
            {
                left = 89;
                center = 233;
                right = 144;
            }

            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + left;
            triangles[i * 6 + 2] = i + center;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + center;
            triangles[i * 6 + 5] = i + right;
        }

        /*for (int i = 0; i < 34; i += 1)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + 13;
            triangles[i * 6 + 2] = i + 21;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 21;
            triangles[i * 6 + 5] = i + 8;
        }
        for (int i = 34; i < 144; i += 1)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + 34;
            triangles[i * 6 + 2] = i + 55;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 55;
            triangles[i * 6 + 5] = i + 21;
        }
        for (int i = 144; i < 1597; i += 1)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + 34;
            triangles[i * 6 + 2] = i + 89;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 89;
            triangles[i * 6 + 5] = i + 55;
        }
        for (int i = 1597; i < 2584; i += 1)
        {
            triangles[i * 6] = i;
            triangles[i * 6 + 1] = i + 89;
            triangles[i * 6 + 2] = i + 233;

            triangles[i * 6 + 3] = i;
            triangles[i * 6 + 4] = i + 233;
            triangles[i * 6 + 5] = i + 144;
        }
        for (int i = 2584; i < numberOfPoints - 2584; i+=1)
        {
            triangles[i*6] = i;
            triangles[i*6 + 1] = i+89;
            triangles[i*6 + 2] = i+322;

            triangles[i*6 + 3] = i;
            triangles[i*6 + 4] = i + 322;
            triangles[i*6 + 5] = i + 233;
        }*/

        return triangles;
    }
}
