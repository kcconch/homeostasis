using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class moving_forward : MonoBehaviour
{
    public GameObject CurrentCamera;


    void Update()
    {
        if (CurrentCamera == null)
        return;

        transform.Translate(Vector3.forward * Time.deltaTime * 2);
    }
}

