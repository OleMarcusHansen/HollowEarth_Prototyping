using UnityEngine;

public class Orienting : MonoBehaviour
{
    GravityRepulsor firmament;
    GravityAttractor moon;
    GravityAttractor sun;

    private void Awake()
    {
        firmament = GameObject.FindGameObjectWithTag("Firmament").GetComponent<GravityRepulsor>();
        moon = GameObject.FindGameObjectWithTag("Moon").GetComponent<GravityAttractor>();
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<GravityAttractor>();
    }

    private void Update()
    {
        firmament.Orient(transform);
        moon.Orient(transform);
        sun.Orient(transform);
    }
}
