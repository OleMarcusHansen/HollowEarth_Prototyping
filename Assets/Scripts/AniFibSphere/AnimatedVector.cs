using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedVector
{
    public Vector3 point, target;
    public AnimatedVector(Vector3 p, Vector3 t)
    {
        point = p;
        target = t;
    }
    public void Animate(float speed)
    {
        Vector3 dir = target - point;
        float magnitude = dir.magnitude;
        float targetMagnitude = speed * Time.deltaTime;
        targetMagnitude = Mathf.Clamp(targetMagnitude, 0, magnitude);
        Vector3 endPoint = point + dir * targetMagnitude;
        point = endPoint;
    }
}
