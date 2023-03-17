using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Parametros")]
    public float rotationSpeed;
    void Update()
    {
        RotateObject();
    }

    void RotateObject()
    {
        this.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}
