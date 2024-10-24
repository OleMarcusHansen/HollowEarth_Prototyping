using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Gravity : MonoBehaviour
{
    GravityRepulsor firmament;
    GravityAttractor moon;
    GravityAttractor sun;
    Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        if (firmament != null)
        {
            firmament.Attract(rb);
        }

        if (moon != null)
        {
            moon.Attract(rb);
        }
        if (sun != null)
        {
            sun.Attract(rb);
        }
    }
}
