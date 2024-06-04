
using System;
using UnityEngine;

public class ArmBobbing : MonoBehaviour {
    // Sway
    float step = 0.01f;
    float maxStepDistance = 0.06f;
    float rotationStep = 4f;
    float maxRotationStep = 5;
    float smooth = 10f;
    float smoothRot = 12f;
    Vector3 swayPos;
    Vector3 swayEulerRot;
    Vector2 lookInput;

    float rotationX;
    float rotationY;

    [SerializeField] PlayerMovement pm;
    Rigidbody rb;

    // Bobbing
    public float speedCurve;
    float curveSin { get => Mathf.Sin(speedCurve); }
    float curveCos { get => Mathf.Cos(speedCurve); }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    public Vector2 walkInput;
    public Vector3 multiplier;
    Vector3 bobEulerRotation;
    Vector3 bobPosition;


    void Start() {

        rb = pm.GetComponent<Rigidbody>();
        lookInput = PlayerCamera.lookInput;

    }

    // Update is called once per frame
    void Update() {
        GetMouseInput();
        GetKeyInput();
        Sway();
        SwayRotation();

        BobOffset();
        BobRotation();

        CompositePosRot();

        //Debug.Log(swayPos);
        //Debug.Log(swayEulerRot);
        //Debug.Log(lookInput);

    }

    void Sway() {

        Vector3 invertLook = lookInput * -step;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPos = invertLook;
    }

    void SwayRotation() {

        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);


    }

    void CompositePosRot() {

        transform.localPosition =
            Vector3.Lerp(transform.localPosition,
            swayPos + bobPosition,
            Time.deltaTime * smooth);

        transform.localRotation =
            Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEulerRotation),
            Time.deltaTime * smoothRot);


    }

    void BobOffset() {

        speedCurve += Time.deltaTime * (pm.isGrounded ? rb.velocity.magnitude : 1f) + 0.01f;


        bobPosition.x =
            (curveCos * bobLimit.x * (pm.isGrounded ? 1 : 0))
            - (walkInput.x * travelLimit.x);

        bobPosition.y =
            (curveSin * bobLimit.y)
            - (rb.velocity.y * travelLimit.y);

        bobPosition.z =
            - (walkInput.y * travelLimit.z);

    }


    void BobRotation() {

       

        bobEulerRotation.x = walkInput != Vector2.zero ? multiplier.x * Mathf.Sin(2 * speedCurve) :
            multiplier.x * (Mathf.Sin(2 * speedCurve) / 2);
        bobEulerRotation.y = walkInput != Vector2.zero ? multiplier.y * curveCos : 0;
        bobEulerRotation.z = walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0;


    }

    void GetMouseInput() {

        float mouseX = Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X");
        float mouseY = Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y");

        // Replace this if other var ^ can be changed easily
        lookInput.x = mouseX;
        lookInput.y = mouseY;


    }

    void GetKeyInput() {

        walkInput.x = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal Controller");
        walkInput.y = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Vertical Controller");

    }

}
