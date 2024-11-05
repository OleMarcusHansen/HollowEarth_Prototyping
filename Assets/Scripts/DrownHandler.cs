using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DrownHandler : MonoBehaviour
{
    float timer;
    [SerializeField] int drowntime = 2;
    [SerializeField] GameObject drownedText;
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject monster;

    [SerializeField] GameObject waterAmbience;

    void Update()
    {
        if (transform.position.magnitude > 51.6f)
        {
            //druknelyd
            waterAmbience.SetActive(true);

            timer += Time.deltaTime;

            if (timer > drowntime)
            {
                drownedText.SetActive(true);
                playerController.enabled = false;
                playerController.GetComponent<Rigidbody>().isKinematic = true;
                monster.SetActive(false);
                Invoke(nameof(SendToMenu), 4);
            }
        }
        else if (timer != 0)
        {
            waterAmbience.SetActive(false);
            timer = 0;
        }
    }

    void SendToMenu()
    {
        SceneManager.LoadScene("SpookyMainMenu");
    }
}
