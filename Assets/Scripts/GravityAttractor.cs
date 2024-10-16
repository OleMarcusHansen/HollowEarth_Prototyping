using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    float G = 0.01f;
    [SerializeField] float MASS = 50000;
    [SerializeField] float minDistance = 20;
    [SerializeField] float groundDistance = 50;
    [SerializeField] float maxDistance = 100;
    [SerializeField] float groundGravity = -2;

    float force = 0;

    public void Attract(Rigidbody body)
    {
        Vector3 direction = (body.position - transform.position);
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            //print("too far to attract");
            return;
        }
        else if (distance > minDistance)
        {
            force = body.mass * groundGravity * Mathf.InverseLerp(maxDistance, groundDistance, distance);
        }
        else
        {
            Debug.Log("body too close");
            //body.transform.position = transform.position;
            //body.velocity = Vector3.zero;
            return;
        }

        direction = direction.normalized;

        body.AddForce(force * direction);
    }
    public void Orient(Transform body, bool instant = false)
    {
        Vector3 direction = (body.position - transform.position);
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            //print("too far to orient");
            return;
        }

        direction = direction.normalized;
        //body.rotation = Quaternion.FromToRotation(body.up, direction) * body.rotation;

        Quaternion targetRotation = Quaternion.FromToRotation(body.up, direction) * body.rotation;

        if (instant)
        {
            body.rotation = targetRotation;
        }
        else
        {
            body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Mathf.Abs(force) / 5 * Time.deltaTime);
        }

    }
}
