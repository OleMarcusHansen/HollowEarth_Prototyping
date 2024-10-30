using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KillSphere : MonoBehaviour
{
    [SerializeField] Animator deathCam;
    [SerializeField] GameObject deathScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().enabled = false;

            Camera.main.enabled = false;
            deathCam.gameObject.SetActive(true);
            Invoke(nameof(OpenDeathScreen), 2);
        }
    }

    void OpenDeathScreen()
    {
        deathScreen.SetActive(true);
        Invoke(nameof(SendToMenu), 2);
    }

    void SendToMenu()
    {
        SceneManager.LoadScene("SpookyMainMenu");
    }
}
