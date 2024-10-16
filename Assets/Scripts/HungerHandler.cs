using UnityEngine;

public class HungerHandler : MonoBehaviour
{
    PlayerStats playerStats;
    float starveSpeed = 0.1f;

    bool healing = false;
    bool walking = false;
    bool running = false;
    bool freezing = false;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        playerStats.hunger -= Time.deltaTime * starveSpeed * 0.1f;
    }

    public void SetHealing(bool b)
    {
        if (healing != b)
        {
            healing = b;

            if (healing == true)
                starveSpeed += 0.1f;
            else
                starveSpeed -= 0.1f;
        }
    }
    public void SetWalking(bool b)
    {
        if (walking != b)
        {
            walking = b;

            if (walking == true)
                starveSpeed += 0.1f;
            else
                starveSpeed -= 0.1f;
        }
    }
    public void SetRunning(bool b)
    {
        if (running != b)
        {
            running = b;

            if (running == true)
                starveSpeed += 0.1f;
            else
                starveSpeed -= 0.1f;
        }
    }
    public void SetFreezing(bool b)
    {
        if (freezing != b)
        {
            freezing = b;

            if (freezing == true)
                starveSpeed += 0.1f;
            else
                starveSpeed -= 0.1f;
        }
    }
}
