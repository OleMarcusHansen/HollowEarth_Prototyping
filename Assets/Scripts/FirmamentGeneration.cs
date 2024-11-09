using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DelaunatorSharp;
using TMPro;

public class FirmamentGeneration : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] ShapeSettings shapeSettings = new ShapeSettings();

    [Header("Terrain Settings")]
    [SerializeField] NoiseSettings ridgeSettings = new NoiseSettings();
    RidgeNoiseFilter ridgeFilter;
    [SerializeField] NoiseSettings landSettings = new NoiseSettings();
    SimpleNoiseFilter landFilter;
    [SerializeField] NoiseSettings mountainSettings = new NoiseSettings();
    SimpleNoiseFilter mountainFilter;
    [SerializeField] NoiseSettings oceanSettings = new NoiseSettings();
    SimpleNoiseFilter oceanFilter;

    [Header("Nature Settings")]
    [SerializeField] NatureSettings birchSettings = new NatureSettings();
    NatureFilter birchFilter;
    [SerializeField] NatureSettings spruceSettings = new NatureSettings();
    NatureFilter spruceFilter;
    [SerializeField] NatureSettings spawnbushSettings = new NatureSettings();
    NatureFilter spawnbushFilter;
    [SerializeField] NatureSettings stonesSettings = new NatureSettings();
    NatureFilter stonesFilter;
    [SerializeField] NatureSettings berrybushSettings = new NatureSettings();
    NatureFilter berrybushFilter;
    [SerializeField] NatureSettings redfruittreeSettings = new NatureSettings();
    NatureFilter redfruittreeFilter;
    [SerializeField] NatureSettings palmtreeSettings = new NatureSettings();
    NatureFilter palmtreeFilter;
    [SerializeField] NatureSettings swamptreeSettings = new NatureSettings();
    NatureFilter swamptreeFilter;

    [SerializeField] NatureGroup[] natureGroups;

    [Header("Ground Material Settings")]
    [SerializeField] NatureSettings grassSettings = new NatureSettings();
    NatureFilter grassFilter;
    [SerializeField] NatureSettings snowSettings = new NatureSettings();
    NatureFilter snowFilter;
    [SerializeField] NatureSettings sandSettings = new NatureSettings();
    NatureFilter sandFilter;
    [SerializeField] NatureSettings claySettings = new NatureSettings();
    NatureFilter clayFilter;
    [SerializeField] NatureSettings stoneSettings = new NatureSettings();
    NatureFilter stoneFilter;

    [Header("Tiles")]
    [SerializeField] TileManager tileManager;

    //Vector3[] vertices;
    //int[] triangles;
    //Color[] colors;
    Vector3[] normals;

    [SerializeField] Color[] groundMaterialColors;

    [Header("Seed")]
    [SerializeField] string seed;
    [SerializeField] bool useSeed;
    [SerializeField] TextMeshProUGUI text;

    float ridgeDeepness;

    private void Start()
    {
        StartCoroutine(GenerateFirmament());
    }

    IEnumerator GenerateFirmament()
    {
        //float startTime = Time.realtimeSinceStartup;
        //Debug.Log("Starting firmament generation");
        //yield return null;

        GenData genData = FindObjectOfType<GenData>();

        if (genData != null)
        {
            text.text = "Seed: " + genData.seed.ToString();
            Debug.Log("seed used: " + genData.seed);
            Debug.Log("Hashed seed: " + genData.seed.GetHashCode().ToString());
            Random.InitState(genData.seed.GetHashCode());
        }
        else
        {
            if (useSeed)
            {
                text.text = "Seed: " + seed.ToString();
                Debug.Log("Hashed seed: " + seed.GetHashCode().ToString());
                Random.InitState(seed.GetHashCode());
            }
            else
            {
                string randomSeed = Random.Range(0, 100000).ToString();
                Debug.Log("Random seed used: " + randomSeed);
                Debug.Log("Hashed seed: " + randomSeed.GetHashCode().ToString());
                if (text != null)
                {
                    text.text = "Seed: " + randomSeed.ToString();
                }
                Random.InitState(randomSeed.GetHashCode());
            }
        }


        ridgeFilter = new RidgeNoiseFilter(ridgeSettings);
        landFilter = new SimpleNoiseFilter(landSettings);
        mountainFilter = new SimpleNoiseFilter(mountainSettings);
        oceanFilter = new SimpleNoiseFilter(oceanSettings);

        birchFilter = new NatureFilter(birchSettings);
        spruceFilter = new NatureFilter(spruceSettings);
        spawnbushFilter = new NatureFilter(spawnbushSettings);
        stonesFilter = new NatureFilter(stonesSettings);
        berrybushFilter = new NatureFilter(berrybushSettings);
        redfruittreeFilter = new NatureFilter(redfruittreeSettings);
        palmtreeFilter = new NatureFilter(palmtreeSettings);
        swamptreeFilter = new NatureFilter(swamptreeSettings);

        for (int i = 0; i < natureGroups.Length; i++)
        {
            natureGroups[i].filter = new NatureFilter(natureGroups[i].settings);

            natureGroups[i].biasTotal = 0;
            foreach (NatureGroup.NatureItem nItem in natureGroups[i].natureItems)
            {
                natureGroups[i].biasTotal += nItem.bias;
            }
        }

        grassFilter = new NatureFilter(grassSettings);
        snowFilter = new NatureFilter(snowSettings);
        sandFilter = new NatureFilter(sandSettings);
        clayFilter = new NatureFilter(claySettings);
        stoneFilter = new NatureFilter(stoneSettings);

        //Create mesh info
        Fibonator fibonator = new Fibonator(shapeSettings.numberOfPoints, shapeSettings.radius, shapeSettings.x1, shapeSettings.x2);
        tileManager.vertices = fibonator.Points;

        //Debug.Log("Finished point calculations in " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;

        //Create half mesh info
        /*
        Vector3[] tempPoints = FibonacciPoints(shapeSettings.numberOfPoints);
        points = new Vector3[tempPoints.Length / 2];
        System.Array.Copy(tempPoints, points, tempPoints.Length / 2);
        */

        tileManager.triangles = Triangulator.DelaunayTriangles(tileManager.vertices, shapeSettings.radius, true);
        

        //Debug.Log("Finished triangle calculations in " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;

        tileManager.colors = new Color[tileManager.vertices.Length];

        tileManager.tileDatas = new TileData[tileManager.vertices.Length];

        ridgeDeepness = oceanSettings.strength * (1 - oceanSettings.minValue) / -2;
        //Add noise
        for (int i = 0; i < tileManager.vertices.Length; i++)
        {
            UpdateElevation(i);
        }

        //Debug.Log("Finished terrain noise calculations in " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;
        
        //Set up mesh
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = tileManager.vertices;
        mesh.triangles = tileManager.triangles;
        mesh.RecalculateNormals();
        
        //Debug.Log("Finished calculating normals " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;


        normals = mesh.normals;
        for (int i = 0; i < tileManager.vertices.Length; i++)
        {
            UpdateTileData(i);
        }

        //Debug.Log("Finished material and nature noise calculations in " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;

        mesh.colors = tileManager.colors;
        
        GetComponent<MeshFilter>().mesh = mesh;

        //Debug.Log("Finished assigning mesh in " + (Time.realtimeSinceStartup - startTime));
        //yield return null;
        //startTime = Time.realtimeSinceStartup;

        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        //Debug.Log("Finished assigning collider in " + (Time.realtimeSinceStartup - startTime));

        yield return null;
    }

    void UpdateElevation(int i)
    {
        // Elevation
        float elevation = 0;
        float oceanValue = oceanFilter.Evaluate(tileManager.vertices[i]);
        elevation += oceanValue;
        float ridgeValue = ridgeFilter.Evaluate(tileManager.vertices[i]);
        elevation += ridgeValue * ((ridgeDeepness + oceanValue) / ridgeDeepness); // fix this calculation thing
        elevation += landFilter.Evaluate(tileManager.vertices[i]);
        elevation += mountainFilter.Evaluate(tileManager.vertices[i]);
        elevation = Mathf.Round(elevation * 4) / 4f;
        tileManager.vertices[i] -= elevation * tileManager.vertices[i].normalized;
    }

    Vector3 FindRotation(int i)
    {
        Vector3 direction = (tileManager.vertices[i] - transform.position);

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -direction) * transform.rotation;

        return targetRotation.eulerAngles;
    }

    void UpdateTileData(int i)
    {
        tileManager.tileDatas[i] = new TileData(tileManager.vertices[i], FindRotation(i), GroundMaterial.Dirt);

        float oceanValue = oceanFilter.Evaluate(tileManager.vertices[i]);
        float ridgeValue = ridgeFilter.Evaluate(tileManager.vertices[i]);

        // Ground Materials

        if (ridgeValue > 0)
        {
            if (stoneFilter.Evaluate(tileManager.vertices[i]))
            {
                tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Stone);
            }
            else
            {
                Vector3 normalizedNormal = normals[i].normalized;
                Vector3 normalizedPoint = tileManager.vertices[i].normalized;
                if ((normalizedNormal.x * normalizedPoint.x) + (normalizedNormal.y * normalizedPoint.y) + (normalizedNormal.z * normalizedPoint.z) > -0.8f)
                {
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Stone);
                }
            }
        }

        if (tileManager.tileDatas[i].groundMaterial[0] != GroundMaterial.Stone)
        {
            if (oceanValue < 0f)
            {
                if (clayFilter.Evaluate(tileManager.vertices[i]))
                {
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Clay);
                }
                else if (sandFilter.Evaluate(tileManager.vertices[i]))
                {
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Sand);
                }
            }

            if (snowFilter.Evaluate(tileManager.vertices[i]))
            {
                tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Snow);
            }
            else if (grassFilter.Evaluate(tileManager.vertices[i]))
            {
                if (tileManager.tileDatas[i].groundMaterial[0] != GroundMaterial.Clay)
                {
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Grass);
                }
            }

            if (tileManager.tileDatas[i].groundMaterial[0] == GroundMaterial.Dirt || tileManager.tileDatas[i].groundMaterial[0] == GroundMaterial.Grass)
            {
                if (tileManager.vertices[i].magnitude >= shapeSettings.radius) // change to mudfilter
                {
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Mud);
                }
            }

            // Vegetation



            foreach (NatureGroup natureGroup in natureGroups)
            {
                if ((int)tileManager.tileDatas[i].groundMaterial[0] <= natureGroup.groundMaterialMaxIndex)
                {
                    if (natureGroup.filter.Evaluate(tileManager.vertices[i]))
                    {
                        float rand = Random.value * natureGroup.biasTotal;
                        int biasCounter = 0;
                        foreach (NatureGroup.NatureItem nItem in natureGroup.natureItems)
                        {
                            biasCounter += nItem.bias;
                            if (rand < biasCounter)
                            {
                                tileManager.tileDatas[i].itemSlot = new ItemData(nItem.item, Random.Range(0, 360));

                                if (nItem.item == ItemId.Spawnbush)
                                {
                                    tileManager.specialTiles.Add(i);
                                }

                                break;
                            }
                        }
                    }
                }
            }


            /*
        if ((int)tileManager.tileDatas[i].groundMaterial[0] < 4)
        {
            float rand = Random.value;

            if (spawnbushFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.9f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.Spawnbush, Random.Range(0, 360));
                    tileManager.specialTiles.Add(i);
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
            }
            else if (birchFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.15f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.LeafTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.45f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.LeafTreeSmall, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.65f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.LeafTreeLarge, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.775f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
                else if (rand < 0.89f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
            else if (spruceFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.15f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PineTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.45f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PineTreeSmall, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.65f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PineTreeLarge, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.775f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
                else if (rand < 0.89f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
            else if (berrybushFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BerryBush, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.9f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }

            }
            else if (redfruittreeFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.3f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.RedfruitTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.RedfruitTree, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
            }
            else if (palmtreeFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.4f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PalmTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.74f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PalmTreeSmall, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.8f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PalmTreeLarge, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Dirt);
                }
                else if (rand < 0.9f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
            else if (swamptreeFilter.Evaluate(tileManager.vertices[i]))
            {
                if (rand < 0.35f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.SwampTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Mud);
                }
                else if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.SwampTree, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Mud);
                }
                else if (rand < 0.84f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
                else if (rand < 0.96f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
        }
        else if (tileManager.tileDatas[i].groundMaterial[0] == GroundMaterial.Clay)
        {
            if (swamptreeFilter.Evaluate(tileManager.vertices[i]))
            {
                float rand = Random.value;
                if (rand < 0.35f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.SwampTreeYoung, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Mud);
                }
                else if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.SwampTree, Random.Range(0, 360));
                    tileManager.tileDatas[i].SetGroundMaterial(GroundMaterial.Mud);
                }
                else if (rand < 0.84f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickThin, Random.Range(0, 360));
                }
                else if (rand < 0.96f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
        }
        // Stones
        if (stonesFilter.Evaluate(tileManager.vertices[i]))
        {
            float rand = Random.value;
            if (rand < 0.5f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneSmall, Random.Range(0, 360));
            }
            else if (rand < 0.8f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneLarge, Random.Range(0, 360));
            }
            else if (rand < 0.95f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderSmall, Random.Range(0, 360));
            }
            else
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderLarge, Random.Range(0, 360));
            }
        }
        */

    }

        /*
        // plan and fix order and stuff of all of this
        if (grassFilter.Evaluate(tileManager.vertices[i]))
        {
            tileManager.tileDatas[i].groundMaterial = GroundMaterial.Grass;
        }
        if (oceanValue < 0f)
        {
            if (sandFilter.Evaluate(tileManager.vertices[i]))
            {
                tileManager.tileDatas[i].groundMaterial = GroundMaterial.Sand;
            }
        }
        if (snowFilter.Evaluate(tileManager.vertices[i]))
        {
            tileManager.tileDatas[i].groundMaterial = GroundMaterial.Snow;
        }

        if (ridgeValue > 0)
        {
            if (tileManager.tileDatas[i].groundMaterial == GroundMaterial.Sand)
            {
                tileManager.tileDatas[i].groundMaterial = GroundMaterial.Stone;
            }
            else
            {
                Vector3 normalizedNormal = normals[i].normalized;
                Vector3 normalizedPoint = tileManager.vertices[i].normalized;
                if ((normalizedNormal.x * normalizedPoint.x) + (normalizedNormal.y * normalizedPoint.y) + (normalizedNormal.z * normalizedPoint.z) > -0.75f)
                {
                    tileManager.tileDatas[i].groundMaterial = GroundMaterial.Stone;
                }
            }
        }
        if (clayFilter.Evaluate(tileManager.vertices[i]))
        {
            tileManager.tileDatas[i].groundMaterial = GroundMaterial.Clay;
        }

        if (tileManager.tileDatas[i].groundMaterial == GroundMaterial.Dirt || tileManager.tileDatas[i].groundMaterial == GroundMaterial.Grass)
        {
            if (tileManager.vertices[i].magnitude > shapeSettings.radius) // change to mudfilter
            {
                tileManager.tileDatas[i].groundMaterial = GroundMaterial.Mud;
            }
        }

        // Trees
        if ((int)tileManager.tileDatas[i].groundMaterial < 4)
        {
            if (spawnbushFilter.Evaluate(tileManager.vertices[i]))
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.Spawnbush, Random.Range(0, 360));
                tileManager.specialTiles.Add(i);
                tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
            }
            else if (birchFilter.Evaluate(tileManager.vertices[i]))
            {
                float rand = Random.value;
                if (rand < 0.5f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.LeafTreeSmall, Random.Range(0, 360));
                    tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
                }
                else if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.LeafTreeLarge, Random.Range(0, 360));
                    tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
                }
                else if (rand < 0.9f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
            else if (spruceFilter.Evaluate(tileManager.vertices[i]))
            {
                float rand = Random.value;
                if (rand < 0.5f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PineTreeSmall, Random.Range(0, 360));
                    tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
                }
                else if (rand < 0.7f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.PineTreeLarge, Random.Range(0, 360));
                    tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
                }
                else if (rand < 0.9f)
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickSmall, Random.Range(0, 360));
                }
                else
                {
                    tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StickLarge, Random.Range(0, 360));
                }
            }
            else if (berrybushFilter.Evaluate(tileManager.vertices[i]))
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BerryBush, Random.Range(0, 360));
                tileManager.tileDatas[i].groundMaterial = GroundMaterial.Dirt;
            }
        }
        if (stonesFilter.Evaluate(tileManager.vertices[i]))
        {
            float rand = Random.value;
            if (rand < 0.5f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneSmall, Random.Range(0, 360));
            }
            else if (rand < 0.8f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.StoneLarge, Random.Range(0, 360));
            }
            else if (rand < 0.95f)
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderSmall, Random.Range(0, 360));
            }
            else
            {
                tileManager.tileDatas[i].itemSlot = new ItemData(ItemId.BoulderLarge, Random.Range(0, 360));
            }
        }
        */
        tileManager.colors[i] = groundMaterialColors[((int)tileManager.tileDatas[i].groundMaterial[0])];
    }
}

