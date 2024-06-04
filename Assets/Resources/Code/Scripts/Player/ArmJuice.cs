using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmBobbing : MonoBehaviour
{

    float step = 0.01f; 
    float maxStepDistance = 0.06f;
    float rotationStep = 4f;
    float maxRotationStep = 5;
    Vector3 swayPos;
    Vector3 swayEulerRot; 
    


    // Update is called once per frame
    void Update()
    {
        Sway();
        SwayRotation();

    }


     void Sway() {
        
        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation() {
        
        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);


    }
}
