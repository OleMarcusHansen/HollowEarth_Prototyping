using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] float speed = .1f;

    void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * speed);
    }
}
