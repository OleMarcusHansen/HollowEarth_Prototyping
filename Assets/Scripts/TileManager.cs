using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TileManager : MonoBehaviour
{
    [HideInInspector] public TileData[] tileDatas;
    [HideInInspector] bool[] tileLoaded;

    List<TileObject> tileObjects;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform parent;

    [SerializeField] ItemMapping itemMap;
    [SerializeField] GroundMaterialMapping groundMaterialMap;

    [SerializeField] Transform player;
    [SerializeField] GameObject markerPrefab;

    [SerializeField] int loadDistance = 50;
    public int renderDistance = 500;

    [SerializeField] int diameter = 3142;
    int counter = 0;

    [SerializeField] int radius = 500;
    [SerializeField] bool hollow;

    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    [HideInInspector] public Vector3[] vertices;
    [HideInInspector] public int[] triangles;
    [HideInInspector] public Color[] colors;

    [HideInInspector] public List<int> specialTiles = new List<int>(); //spawnpoints and stuff

    public bool loading;

    [SerializeField] ObjectPool tilePool;

    [SerializeField] GameObject loadingText;

    public IEnumerator UpdateTile(int index, TileObject tileObject, float elevationChange, GroundMaterial? groundMaterialChange = null)
    {
        if (elevationChange != 0)
        {
            vertices[index] += elevationChange * vertices[index].normalized;
            tileDatas[index].position = vertices[index];
            tileObject.transform.localPosition = vertices[index];

            //yield return LoadMesh();
            //yield return ReloadCollider();

            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] meshVertices = meshCollider.sharedMesh.vertices;

            meshVertices[tileObjects.IndexOf(tileObject)] = vertices[index];

            mesh.vertices = meshVertices;
            meshCollider.sharedMesh = mesh;

            if (elevationChange > 0)
            {
                if (tileDatas[index].groundMaterial.Count > 1)
                {
                    tileDatas[index].groundMaterial.RemoveAt(tileDatas[index].groundMaterial.Count - 1);
                    colors[index] = groundMaterialMap.GetGroundMaterialDefinition(tileDatas[index].groundMaterial[tileDatas[index].groundMaterial.Count - 1]).color;
                }
            }
        }
        if (groundMaterialChange != null)
        {
            if (elevationChange < 0)
            {
                tileDatas[index].groundMaterial.Add((GroundMaterial)groundMaterialChange);
            }
            else
            {
                tileDatas[index].groundMaterial[tileDatas[index].groundMaterial.Count-1] = (GroundMaterial)groundMaterialChange;
            }
            colors[index] = groundMaterialMap.GetGroundMaterialDefinition((GroundMaterial)groundMaterialChange).color;
        }

        yield return LoadMesh();

        yield return null;
    }

    //happens on start and after terraforming
    IEnumerator LoadMesh()
    {
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = vertices;
        mesh.colors = colors;

        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        yield return null;
        Debug.Log("finished load mesh");
    }

    //happens after terraforming
    IEnumerator ReloadCollider()
    {
        yield return null;
        Debug.Log("start reload collider");
        Mesh mesh = new Mesh();
        Vector3[] meshVertices = new Vector3[tileObjects.Count];
        int[] meshTriangles;

        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshVertices[i] = tileDatas[tileObjects[i].id].position;
        }

        yield return null;
        Debug.Log("finished finding verts");

        meshTriangles = meshCollider.sharedMesh.triangles;

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        meshCollider.sharedMesh = mesh;

        yield return null;
        Debug.Log("finished reload collider");
    }

    //happens on start and load
    public IEnumerator LoadCollider(bool async = true)
    {
        /*
        Debug.Log("start collider load");
        yield return null;
        float startTime = Time.realtimeSinceStartup;*/

        Mesh mesh = new Mesh();
        Vector3[] meshVertices = new Vector3[tileObjects.Count];
        //int[] meshTriangles;
        List<int> meshTriangles = new List<int>();

        /*Debug.Log("Finished variable setup " + (Time.realtimeSinceStartup - startTime));
        yield return null;
        startTime = Time.realtimeSinceStartup;*/

        for (int i = 0; i < meshVertices.Length; i++)
        {
            meshVertices[i] = tileDatas[tileObjects[i].id].position;
        }

        /*Debug.Log("Finished finding verts in " + (Time.realtimeSinceStartup - startTime));
        yield return null;
        startTime = Time.realtimeSinceStartup;*/

        //meshTriangles = Triangulator.DelaunayTriangles(meshVertices, radius, hollow);

        //foreach triangle
        //if triangle distance below 25 or 30
        //see if it (all three points) exists in any tileObject.id
        //add triangle (3 points with the new indexes)

        for (int i = 0; i < triangles.Length; i+=3)
        {
            if (tileLoaded[triangles[i]] && tileLoaded[triangles[i + 1]] && tileLoaded[triangles[i + 2]])
            {
                for (int j = 0; j < tileObjects.Count; j++)
                {
                    if (tileObjects[j].id == triangles[i])
                    {
                        meshTriangles.Add(j);
                    }
                }
                for (int j = 0; j < tileObjects.Count; j++)
                {
                    if (tileObjects[j].id == triangles[i + 1])
                    {
                        meshTriangles.Add(j);
                    }
                }
                for (int j = 0; j < tileObjects.Count; j++)
                {
                    if (tileObjects[j].id == triangles[i + 2])
                    {
                        meshTriangles.Add(j);
                    }
                }

                if (i % 250 == 0 && async == true)
                {
                    yield return null;
                }
            }
        }

        /*Debug.Log("Finished finding triangles in " + (Time.realtimeSinceStartup - startTime));
        yield return null;
        startTime = Time.realtimeSinceStartup;*/

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles.ToArray();
        meshCollider.sharedMesh = mesh;

        yield return null;
    }

    public IEnumerator LoadTiles(bool async = true)
    {
        if (loading == true && async == false)
        {
            Debug.LogWarning("Tried loading tiles while already loading tiles");
            yield break;
        }
        loading = true;

        if (async == false)
        {
            loadingText.SetActive(true);
            yield return null;
        }

        Debug.Log("Starting to load tiles " + gameObject.name);

        int mid = TileDataNearPosition(player.position.y);

        if (tileObjects == null)
        {
            tileObjects = new List<TileObject>();
            tileLoaded = new bool[tileDatas.Length];
        }

        float dist;

        // Unload tiles
        for (int j = 0, c = 0; j < tileObjects.Count; j++, c++)
        {
            dist = Vector3.Distance(parent.TransformPoint(tileDatas[tileObjects[j].id].position), player.position);

            if (dist > loadDistance)
            {
                tileLoaded[tileObjects[j].id] = false;
                tilePool.DeactivateInPool(tileObjects[j].gameObject);
                tileObjects[j].ResetTile();
                /*if (tileObjects[j].itemSlot != null)
                {
                    Destroy(tileObjects[j].itemSlot);
                }*/
                tileObjects.RemoveAt(j);
                j--;

                if (c % 5000 == 0 && async == true)
                {
                    yield return null;
                }
            }
        }

        Debug.Log("Finished deleting tiles in " + gameObject.name);

        // Middle Out to load tiles

        int i = 0;
        counter = 0;
        bool finishedNorth = false;
        bool finishedSouth = false;
        while ((!finishedNorth || !finishedSouth)/* && counter < diameter*/)
        {
            if (mid + i < tileDatas.Length)
            {
                if (counter < diameter)
                {
                    LoadTile(mid + i);
                }
                else
                {
                    UnlodTile(mid + i);
                }
            }
            else
            {
                finishedNorth = true;
            }

            if (mid - 1 - i >= 0)
            {
                if (counter < diameter)
                {
                    LoadTile(mid - 1 - i);
                }
                else
                {
                    UnlodTile(mid - 1 - i);
                }
            }
            else
            {
                finishedSouth = true;
            }

            if (i % 50000 == 0 && async == true)
            {
                yield return null;
            }

            i++;
        }
        Debug.Log("Finished spawning tiles " + gameObject.name);
        if (async == true)
        {
            yield return null;
        }

        if (tileObjects.Count > 2)
        {
            yield return LoadCollider(async);
        }

        Debug.Log("Finished assigning collider " + gameObject.name);
        loading = false;

        yield return null;

        if (async == false)
        {
            loadingText.SetActive(false);
        }
    }

    void LoadTile(int index)
    {
        float dist = Vector3.Distance(parent.TransformPoint(tileDatas[index].position), player.position);
        if (dist < renderDistance)
        {
            counter = 0;
            if (tileLoaded[index] == false)
            {
                if (dist < loadDistance)
                {
                    TileObject tile = tilePool.InstantiateInPool(parent, tileDatas[index].position, tileDatas[index].rotation).GetComponent<TileObject>();
                    tile.id = index;
                    if (tileDatas[index].itemSlot != null) //Endre til noe loop
                    {
                        tile.SpawnItem(itemMap.GetItemDefinition(tileDatas[index].itemSlot.item).prefab, tileDatas[index].itemSlot.rotation);

                        if (tileDatas[index].itemSlot.itemSlot != null)
                        {
                            tile.itemSlot.SpawnItem(itemMap.GetItemDefinition(tileDatas[index].itemSlot.itemSlot.item).prefab, tileDatas[index].itemSlot.itemSlot.rotation);

                        }
                    }
                    tileLoaded[index] = true;
                    tileObjects.Add(tile);

                    if (tileDatas[index].lodObject != null)
                    {
                        Destroy(tileDatas[index].lodObject);
                        tileDatas[index].lodObject = null;
                    }
                }
                else if (tileDatas[index].itemSlot != null)
                {
                    if (tileDatas[index].lodObject == null)
                    {
                        GameObject lodPrefab = itemMap.GetItemDefinition(tileDatas[index].itemSlot.item).lodPrefab;
                        if (lodPrefab != null)
                        {
                            tileDatas[index].lodObject = Instantiate(lodPrefab, parent);
                            tileDatas[index].lodObject.transform.localPosition = tileDatas[index].position;
                            tileDatas[index].lodObject.transform.localEulerAngles += new Vector3(0, Random.Range(0, 360), 0);
                        }
                    }
                }
            }
        }
        else
        {
            counter++;
            UnlodTile(index);
        }
    }
    void UnlodTile(int index)
    {
        if (tileDatas[index].lodObject != null)
        {
            Destroy(tileDatas[index].lodObject);
            tileDatas[index].lodObject = null;
        }
    }

    int TileDataNearPosition(float y)
    {
        int first = 0;
        int last = tileDatas.Length - 1;

        while (first < last)
        {
            int mid = (first + last) / 2;

            if (tileDatas[mid].position.y < y)
            {
                first = mid + 1;
            }
            else
            {
                last = mid;
            }
        }

        return first;
    }
}
