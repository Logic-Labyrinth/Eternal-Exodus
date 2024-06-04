using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionFollower : MonoBehaviour
{
 
    public Transform targetTransform;
    public Vector3 Offset;


    // Update is called once per frame
    void Update()
    {
        transform.position = targetTransform.position + Offset;
    }
}
