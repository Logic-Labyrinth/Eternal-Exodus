using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class Movement : MonoBehaviour {
    public Transform groundCheck;
    public Camera cam;
    public LayerMask groundMask;
    public float normalGravity;
    public float jumpHeight;
    public float runSpeed, airSpeed, crouchSpeed;
    public float crouchCameraTimer;
    public float maxSlideTimer, slideSpeedIncrease, slideSpeedDecrease;
    public float fastFOV;

    CharacterController controller;
    Vector3 move, input, velocityY, forwardDirection;
    // Vector3 crouchingCenter = new Vector3(0f, 0.5f, 0f);
    // Vector3 standingCenter = new Vector3(0f, 0, 0f);
    float speed, gravity, startHeight, slideTimer, normalFOV;
    float crouchHeight = 0.5f;
    int jumpCharges;
    bool isGrounded, isCrouching, isSliding;

    void Start() {
        controller = GetComponent<CharacterController>();
        startHeight = transform.localScale.y;
        normalFOV = cam.fieldOfView;
    }

    void Update() {
        HandleInput();
        CheckGround();
        HandleMovement();
        controller.Move(move * Time.deltaTime);
        ApplyGravity();
        CameraEffects();
    }

    void HandleInput() {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetButtonDown("Jump") && jumpCharges > 0) Jump();
        if (Input.GetButtonDown("Crouch")) Crouch();
        if (Input.GetButtonUp("Crouch")) ExitCrouch();
    }

    void CameraEffects() {
        // float fov = isSliding ? fastFOV : normalFOV;
        // camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, Time.deltaTime * 10f);
        float targetFOV = Mathf.Lerp(normalFOV, fastFOV, controller.velocity.magnitude / runSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 10f);
        CrouchCamera();
    }

    void GroundMovement() {
        speed = isCrouching ? crouchSpeed : runSpeed;
        move.x = input.x == 0 ? 0 : input.x * speed;
        move.z = input.z == 0 ? 0 : input.z * speed;

        move = Vector3.ClampMagnitude(move, speed);
    }

    void AirMovement() {
        speed = airSpeed;
        move.x = input.x == 0 ? 0 : input.x * speed;
        move.z = input.z == 0 ? 0 : input.z * speed;

        move = Vector3.ClampMagnitude(move, speed);
    }

    void ApplyGravity() {
        gravity = normalGravity;
        velocityY.y -= gravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    void CheckGround() {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (isGrounded) {
            jumpCharges = 1;
        }
    }

    void Jump() {
        velocityY.y = Mathf.Sqrt(jumpHeight * 2 * normalGravity);
        jumpCharges--;

        if (isCrouching) ExitCrouch();
    }

    void HandleMovement() {
        if (isGrounded && !isSliding) GroundMovement();
        else if (!isGrounded) AirMovement();
        else if (isSliding) {
            SlideMovement();
            DecreaseSpeed(slideSpeedDecrease);
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0) isSliding = false;
        }
    }

    void Crouch() {
        // Debug.Log("Crouching");
        // float height = Mathf.Lerp(startHeight, crouchHeight, Time.deltaTime * 10f);
        // controller.height = height;
        // controller.center = crouchingCenter;

        // controller.height = crouchHeight;
        // transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        isCrouching = true;
        if (speed > runSpeed * 0.8) {
            isSliding = true;
            forwardDirection = transform.forward;
            if (isGrounded) IncreaseSpeed(slideSpeedIncrease);
            slideTimer = maxSlideTimer;
        }
    }

    void ExitCrouch() {
        // float height = Mathf.Lerp(crouchHeight, startHeight * 2, Time.deltaTime * 10f);
        // controller.height = height;
        // controller.center = standingCenter;

        // controller.height = startHeight * 2;
        // transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);
        isCrouching = false;
        isSliding = false;
    }

    void CrouchCamera() {
        if (isCrouching || isSliding) {
            float height = Mathf.Lerp(startHeight, crouchHeight, Time.deltaTime * 10f);
            controller.height = height;
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
        } else {
            float height = Mathf.Lerp(crouchHeight, startHeight, Time.deltaTime * 10f);
            controller.height = height * 2;
            transform.localScale = new Vector3(transform.localScale.x, height, transform.localScale.z);
        }
    }

    void IncreaseSpeed(float speedIncrease) {
        speed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease) {
        speed -= speedDecrease * Time.deltaTime;
    }

    void SlideMovement() {
        move += forwardDirection;
        move = Vector3.ClampMagnitude(move, speed);
    }
}
