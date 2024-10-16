using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityRepulsor : MonoBehaviour
{
    float G = 0.01f;
    [SerializeField] float MASS = 5972000;
    [SerializeField] float minDistance = 50;
    [SerializeField] float groundDistance = 200;
    [SerializeField] float maxDistance = 500;
    [SerializeField] float groundGravity = 10;

    float force = 0;

    public void Attract(Rigidbody body)
    {
        Vector3 direction = (body.position - transform.position);
        float distance = direction.magnitude;

        if (distance < minDistance)
        {
            //print("too close to repulse");
            return;
        }
        else if (distance < maxDistance)
        {
            force = body.mass * groundGravity * Mathf.InverseLerp(minDistance, groundDistance, distance);
        }
        else
        {
            Debug.Log("body too far away");
            body.transform.position = transform.position;
            body.velocity = Vector3.zero;
            return;
        }

        direction = direction.normalized;

        body.AddForce(force * direction);
    }
    public void Orient(Transform body, bool instant = false)
    {
        Vector3 direction = (body.position - transform.position);
        float distance = direction.magnitude;

        if (distance < minDistance)
        {
            //print("too close to orient");
            return;
        }

        direction = direction.normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(body.up, -direction) * body.rotation;

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
