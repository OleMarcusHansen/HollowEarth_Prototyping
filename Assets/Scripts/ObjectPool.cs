using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] List<GameObject> pool = new List<GameObject>();
    [SerializeField] GameObject prefab;

    public GameObject InstantiateInPool(Transform parent, Vector3 position, Vector3 rotation)
    {
        if (pool.Count > 0)
        {
            GameObject g = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            g.transform.parent = parent;
            g.transform.localPosition = position;
            g.transform.eulerAngles = rotation;
            g.SetActive(true);
            return g;
        }
        else
        {
            GameObject g = Instantiate(prefab, parent);
            g.transform.localPosition = position;
            g.transform.eulerAngles = rotation;
            return g;
        }
    }
    public void DeactivateInPool(GameObject g)
    {
        g.SetActive(false);
        pool.Add(g);
    }

    /*
    public GameObject InstantiateInPool(Vector3 position, Quaternion rotation)
    {
        if (activeObjects < pool.Count)
        {
            foreach (GameObject g in pool) // maybe improve so it doesn't have to go through so many in the pool
            {
                if (!g.activeInHierarchy)
                {
                    g.transform.position = position;
                    g.transform.rotation = rotation;
                    g.SetActive(true);
                    activeObjects++;
                    return g;
                }
            }
        }
        GameObject ga = Instantiate(prefab, position, rotation);
        pool.Add(ga);
        activeObjects++;
        return ga;
    }
    public void DeactivateInPool(GameObject g)
    {
        g.SetActive(false);
        activeObjects--;
    }*/
}
