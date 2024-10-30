using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpookyMenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void EnterGame()
    {
        SceneManager.LoadScene("SpookyWorld");
    }
    public void LeaveGame()
    {
        Application.Quit();
    }
}
