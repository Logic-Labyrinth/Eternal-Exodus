using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Get the main camera
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(mainCamera.transform);
    }
}
