using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenData : MonoBehaviour
{
    public bool customSeed;
    public string seed;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void changeSeed(string s)
    {
        seed = s;
    }
    public void changeCustom(bool b)
    {
        customSeed = b;
    }

    public void LoadScene()
    {
        if (!customSeed)
        {
            seed = Random.Range(0, 100000).ToString();
        }

        SceneManager.LoadScene(1);
    }
}
