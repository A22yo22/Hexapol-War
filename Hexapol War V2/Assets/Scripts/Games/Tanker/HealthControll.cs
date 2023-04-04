using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthControll : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
        GetComponent<Canvas>().worldCamera = cam;
    }

    void Update()
    {
        transform.LookAt(transform.position + cam.transform.forward);
    }
}
