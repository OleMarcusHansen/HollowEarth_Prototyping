using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawn : MonoBehaviour
{
    [SerializeField] int currentFruit = 0;
    [SerializeField] int maxFruit = 1;
    [SerializeField] float spawnChance = 0.1f;
    float tickTime = 1f;

    [SerializeField] GameObject[] fruitVisuals;

    [SerializeField] Interactable interactable;

    private void Start()
    {
        interactable.enabled = false;
        StartCoroutine(CheckSpawn());
    }
    IEnumerator CheckSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime);

            if (currentFruit < maxFruit) //add random based on season
            {
                if (Random.value < spawnChance)
                {
                    SpawnFruit();
                }
            }
        }
    }
    void SpawnFruit()
    {
        fruitVisuals[currentFruit].SetActive(true);
        currentFruit++;
        if (interactable.enabled == false)
        {
            interactable.enabled = true;
        }
    }
    public void DespawnFruit()
    {
        currentFruit--;
        fruitVisuals[currentFruit].SetActive(false);
        if (currentFruit == 0)
        {
            interactable.enabled = false;
        }
    }
}
