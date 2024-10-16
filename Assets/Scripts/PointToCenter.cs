using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToCenter : MonoBehaviour
{
    private void Start()
    {
        Point();
    }
    void OnEnable()
    {
        Point();
    }

    void Point()
    {
        //GravityRepulsor firmament = GameObject.FindGameObjectWithTag("Firmament").GetComponent<GravityRepulsor>();
        if (transform.parent.GetComponent<GravityRepulsor>())
        {
            GravityRepulsor firmament = transform.parent.GetComponent<GravityRepulsor>();
            firmament.Orient(transform, true);
        }
        else if (transform.parent.GetComponent<GravityAttractor>())
        {
            GravityAttractor moon = transform.parent.GetComponent<GravityAttractor>();
            moon.Orient(transform, true);
        }
    }
}
