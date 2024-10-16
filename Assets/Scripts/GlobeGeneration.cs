using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DelaunatorSharp;

public class GlobeGeneration : MonoBehaviour
{
    [SerializeField] ShapeSettings shapeSettings = new ShapeSettings();

    [SerializeField] NoiseSettings terrainSettings = new NoiseSettings();
    SimpleNoiseFilter terrainFilter;

    [SerializeField] NatureSettings stonesSettings = new NatureSettings();
    NatureFilter stonesFilter;

    Mesh mesh;

    [Header("Tiles")]
    [SerializeField] TileManager tileManager;

    //Vector3[] vertices;
    //int[] triangles;
    //Color[] colors;

    [SerializeField] Color[] groundMaterialColors;

    [SerializeField] bool generateMesh = false;
    [SerializeField] bool generateTerrain = false;
    [SerializeField] bool generateTiles = false;


    private void Start()
    {
        generateMesh = true;
        generateTerrain = true;
        //generateTiles = true;
        GenerateGlobe();
    }

    void GenerateGlobe()
    {
        terrainFilter = new SimpleNoiseFilter(terrainSettings);
        stonesFilter = new NatureFilter(stonesSettings);

        //Create mesh info
        Fibonator fibonator = new Fibonator(shapeSettings.numberOfPoints, shapeSettings.radius, shapeSettings.x1, shapeSettings.x2);
        tileManager.vertices = fibonator.Points;

        tileManager.triangles = Triangulator.DelaunayTriangles(tileManager.vertices, shapeSettings.radius, false);

        tileManager.colors = new Color[tileManager.vertices.Length];

        tileManager.tileDatas = new TileData[tileManager.vertices.Length];

        //Add noise
        for (int i = 0; i < tileManager.vertices.Length; i++)
        {
            float elevation = terrainFilter.Evaluate(tileManager.vertices[i]);
            elevation = Mathf.Round(elevation * 4) / 4f;
            tileManager.vertices[i] -= elevation * tileManager.vertices[i].normalized;

            tileManager.tileDatas[i] = new TileData(tileManager.vertices[i], FindRotation(i), GroundMaterial.Stone);

            if (stonesFilter.Evaluate(tileManager.vertices[i]))
            {
                float rand = Random.value;
                if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneSmall, Random.Range(0, 360));
                }
                else if (rand < 0.95f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneLarge, Random.Range(0, 360));
                }
                else if (rand < 0.975f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderLarge, Random.Range(0, 360));
                }
            }

            tileManager.colors[i] = groundMaterialColors[((int)tileManager.tileDatas[i].groundMaterial[0])];
        }

        //Set up mesh
        mesh = new Mesh();
        mesh.vertices = tileManager.vertices;
        mesh.triangles = tileManager.triangles;
        mesh.RecalculateNormals();

        mesh.colors = tileManager.colors;

        GetComponent<MeshFilter>().mesh = mesh;
        
        gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        //MeshExporter.ExportMesh(mesh, Application.persistentDataPath + "/sun.obj");
    }

    Vector3 FindRotation(int i)
    {
        Vector3 direction = (tileManager.vertices[i] - transform.position);

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;

        return targetRotation.eulerAngles;
    }
}
