using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SpookyGameManager : MonoBehaviour
{
    public int pagesCollected;
    [SerializeField] string[] pages;
    [SerializeField] TextMeshProUGUI pagesText;
    [SerializeField] TextMeshProUGUI pageText;

    [SerializeField] GameObject wonText;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject monster;

    [SerializeField] TileManager firmManager;

    public static SpookyGameManager singleton;

    [SerializeField] AudioSource pageSound;

    void Start()
    {
        //spawn pagesTotal pagePrefabs on vacant tiles above sea level
        Invoke(nameof(SpawnPages), 0.1f);
        //open page view of intitial page

        singleton = this;
    }

    void SpawnPages()
    {
        int counter = 0;
        while (counter < pages.Length)
        {
            int randIndex = Random.Range(0, firmManager.tileDatas.Length - 50);
            if (firmManager.tileDatas[randIndex].itemSlot == null && firmManager.tileDatas[randIndex].position.magnitude < 50)
            {
                firmManager.tileDatas[randIndex].itemSlot = new ItemData(ItemId.Page, Random.Range(0, 360));
                counter++;
            }
        }
        //load tiles
        StartCoroutine(firmManager.LoadTiles(false));

        pageText.text = "collect my pages";
        Invoke(nameof(HidePageText), 4);
    }

    public void CollectPage()
    {
        pageText.text = pages[pagesCollected];
        Invoke(nameof(HidePageText), 4);

        pagesCollected++;
        pagesText.text = pagesCollected.ToString();

        pageSound.Play();

        if (pagesCollected >= pages.Length)
        {
            Invoke(nameof(AllPagesCollected), 4);
        }
    }

    void HidePageText()
    {
        pageText.text = "";
    }

    public void AllPagesCollected()
    {
        //skru på lys ved (og åpne?) tunnel?

        wonText.SetActive(true);
        playerController.enabled = false;
        monster.SetActive(false);
        Invoke(nameof(SendToMenu), 4);
    }

    void SendToMenu()
    {
        SceneManager.LoadScene("SpookyMainMenu");
    }
}
