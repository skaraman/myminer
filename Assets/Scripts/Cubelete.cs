using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubelete : MonoBehaviour
{
    public float turnSpeed = 20f;

    void Start()
    {

    }

    void LateUpdate()
    {
        transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
    }

    void OnMouseDown()
    {
        gameObject.SetActive(false);
    }
}
