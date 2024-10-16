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
        firmament = GameObject.FindGameObjectWithTag("Firmament").GetComponent<GravityRepulsor>();
        moon = GameObject.FindGameObjectWithTag("Moon").GetComponent<GravityAttractor>();
        sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<GravityAttractor>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void FixedUpdate()
    {
        firmament.Attract(rb);
        moon.Attract(rb);
        sun.Attract(rb);
    }
}
