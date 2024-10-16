using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TileController : MonoBehaviour
{
    public TileManager firmManager;
    public TileManager moonManager;

    [SerializeField] TextMeshProUGUI text;

    public Vector3 loadedPosition;

    private void Start()
    {
        loadedPosition = transform.position;
        StartCoroutine(CheckDistance());
    }

    IEnumerator CheckDistance()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if (Vector3.Distance(transform.position, loadedPosition) > 25)
            {
                loadedPosition = transform.position;
                StartCoroutine(firmManager.LoadTiles());
                StartCoroutine(moonManager.LoadTiles());
            }
        }
    }

    public void IncreaseLoadDistance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (firmManager.renderDistance < 1000)
            {
                firmManager.renderDistance += 100;
            }
            if (moonManager.renderDistance < 1000)
            {
                moonManager.renderDistance += 100;
            }
        }
        if (text != null)
        {
            text.text = firmManager.renderDistance.ToString();
        }
    }

    public void DecreaseLoadDistance(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (firmManager.renderDistance > 100)
            {
                firmManager.renderDistance -= 100;
            }
            if (moonManager.renderDistance > 100)
            {
                moonManager.renderDistance -= 100;
            }
        }
        if (text != null)
        {
            text.text = firmManager.renderDistance.ToString();
        }
    }

    public void LoadTiles(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            loadedPosition = transform.position;
            StartCoroutine(firmManager.LoadTiles());
            StartCoroutine(moonManager.LoadTiles());
        }
    }
}
