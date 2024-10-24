using UnityEngine;

public class Orienting : MonoBehaviour
{
    GravityRepulsor firmament;
    GravityAttractor moon;
    GravityAttractor sun;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Firmament"))
        {
            firmament = GameObject.FindGameObjectWithTag("Firmament").GetComponent<GravityRepulsor>();
        }
        if (GameObject.FindGameObjectWithTag("Moon"))
        {
            moon = GameObject.FindGameObjectWithTag("Moon").GetComponent<GravityAttractor>();
        }
        if (GameObject.FindGameObjectWithTag("Sun"))
        {
            sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<GravityAttractor>();
        }
    }

    private void Update()
    {
        if (firmament != null)
        {
            firmament.Orient(transform);
        }
        if (moon != null)
        {
            moon.Orient(transform);
        }
        if (sun != null)
        {
            sun.Orient(transform);
        }
    }
}
