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
    public float maxSlideTimer, slideSpeedIncrease, slideSpeedDecrease;
    public float fastFOV;

    CharacterController controller;
    Vector3 move, input, velocityY, forwardDirection, lastPosition;
    float speed, gravity, startHeight, slideTimer, normalFOV;
    readonly float crouchHeight = 0.5f;
    float forwardVelocity = 0f;
    int jumpCharges;
    bool isGrounded, isCrouching, isSliding;
    bool useNormalGravity;

    void Start() {
        controller = GetComponent<CharacterController>();
        startHeight = transform.localScale.y;
        normalFOV = cam.fieldOfView;
        lastPosition = controller.transform.position;
        useNormalGravity = true;
    }

    void Update() {
        HandleInput();
        CheckGround();
        HandleMovement();
        controller.Move(move * Time.deltaTime);
        ApplyGravity();
        CameraEffects();

        CalculateForwardVelocity();
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
        float targetFOV = Mathf.Lerp(normalFOV, fastFOV, forwardVelocity / runSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 10f);
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
        if (useNormalGravity) gravity = normalGravity;
        velocityY.y -= gravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    void CheckGround() {
        // isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f);
        if (isGrounded) jumpCharges = 1;
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
            useNormalGravity = false;
            gravity = 0;
            SlideMovement();
            DecreaseSpeed(slideSpeedDecrease);
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0) {
                isSliding = false;
                useNormalGravity = true;
            }
        }
    }

    void Crouch() {
        controller.height = crouchHeight;
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

        isCrouching = true;
        if (forwardVelocity > runSpeed * 0.8) {
            isSliding = true;
            forwardDirection = transform.forward;
            if (isGrounded) IncreaseSpeed(slideSpeedIncrease);
            slideTimer = maxSlideTimer;
        }
    }

    void ExitCrouch() {
        controller.height = startHeight * 2;
        transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);

        isCrouching = false;
        isSliding = false;
        useNormalGravity = true;
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

    void CalculateForwardVelocity() {
        Vector3 currentVelocity = (controller.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = controller.transform.position;
        forwardVelocity = Vector3.Dot(currentVelocity, controller.transform.forward);
    }
}
