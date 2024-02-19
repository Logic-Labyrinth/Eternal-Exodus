using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [BoxGroup("Settings")]
    [TitleGroup("Settings/Speeds"), SerializeField]
    float runSpeed;

    [TitleGroup("Settings/Speeds"), SerializeField]
    float airSpeed;

    [TitleGroup("Settings/Speeds"), SerializeField]
    float crouchSpeed;

    [TitleGroup("Settings/Speeds"), SerializeField, Range(0f, 1f)]
    float airControl = 1f;

    [TitleGroup("Settings/Slide"), SerializeField]
    float slideCooldown = 2f;

    [TitleGroup("Settings/Slide"), SerializeField]
    float maxSlideTimer = 2f;

    [TitleGroup("Settings/Slide"), SerializeField]
    float slideSpeedIncrease = 2.5f;

    [TitleGroup("Settings/Slide"), SerializeField]
    float slideSpeedDecrease = 3f;

    [TitleGroup("Settings/Jump"), SerializeField]
    float jumpHeight;

    [TitleGroup("Settings/Jump"), SerializeField]
    int jumpCharges = 2;

    [TitleGroup("Settings/Miscellaneous"), SerializeField]
    float fastFOV;

    [TitleGroup("Settings/Miscellaneous"), SerializeField]
    float normalGravity;

    [TitleGroup("Settings/Miscellaneous"), SerializeField]
    float crouchHeight = 0.5f;

    [Space(30)]
    [Title("Extra"), SerializeField]
    Camera cam;

    CharacterController controller;
    Vector3 move,
        input,
        velocityY,
        forwardDirection,
        lastPosition;
    float speed,
        startHeight,
        slideTimer,
        normalFOV;
    float forwardVelocity = 0f;
    float lastSlideTime = Mathf.NegativeInfinity;
    int jumpChargesLeft;
    bool isGrounded,
        isCrouching,
        isSliding,
        wantsToUncrouch;

    private void OnGUI()
    {
        GUILayout.TextArea("Wants to uncrouch: " + wantsToUncrouch);
        GUILayout.TextArea("Crouching: " + isCrouching);
        GUILayout.TextArea("Player pos: " + controller.transform.position);
        GUILayout.TextArea("Fwd vector: " + forwardVelocity);
        GUILayout.TextArea("FOV: " + cam.fieldOfView);
        GUILayout.TextArea("Jump charges: " + (jumpChargesLeft - 1));
        GUILayout.TextArea("Height: " + controller.height);
        GUILayout.TextArea("Start height: " + startHeight);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        startHeight = controller.height;
        normalFOV = cam.fieldOfView;
        lastPosition = controller.transform.position;
        wantsToUncrouch = false;
        lastSlideTime = Time.time;
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleMovement();
        controller.Move(move * Time.deltaTime);
        ApplyGravity();
        CameraEffects();

        CalculateForwardVelocity();
    }

    void HandleInput()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetButtonDown("Crouch"))
            Crouch();
        if (Input.GetButtonUp("Crouch"))
            wantsToUncrouch = true;
    }

    void CameraEffects()
    {
        float targetFOV = Mathf.Lerp(normalFOV, fastFOV, forwardVelocity / runSpeed);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 10f);
    }

    void GroundMovement()
    {
        speed = isCrouching ? crouchSpeed : runSpeed;
        move.x = input.x == 0 ? 0 : input.x * speed;
        move.z = input.z == 0 ? 0 : input.z * speed;

        move = Vector3.ClampMagnitude(move, speed);
    }

    void AirMovement()
    {
        speed = airSpeed;
        move.x = input.x == 0 ? 0 : input.x * speed;
        move.z = input.z == 0 ? 0 : input.z * speed;

        move = Vector3.ClampMagnitude(move * airControl, speed);
    }

    void ApplyGravity()
    {
        velocityY.y -= normalGravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(
            transform.position + (0.5f * controller.height * -Vector3.up),
            -Vector3.up,
            0.2f
        );
        if (isGrounded)
            jumpChargesLeft = jumpCharges;
    }

    private void Jump() {
        if (!isGrounded) return;
        velocityY.y = Mathf.Sqrt(jumpHeight * 2 * normalGravity);
            // if (isCrouching) wantsToUncrouch = true;
    }

    public void SpecialJump() {
        if (jumpChargesLeft <= 1) return;
        velocityY.y = Mathf.Sqrt(jumpHeight * 2 * normalGravity);
        jumpChargesLeft--;
    }

    void HandleMovement()
    {
        TryUncrouch();
        if (isGrounded && !isSliding)
            GroundMovement();
        else if (!isGrounded)
            AirMovement();
        else if (isSliding)
        {
            SlideMovement();
            DecreaseSpeed(slideSpeedDecrease);
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
                isSliding = false;
        }
    }

    void TryUncrouch()
    {
        if (!wantsToUncrouch)
        {
            if (!isCrouching)
                wantsToUncrouch = false;
            return;
        }
        bool checkAbove = Physics.Raycast(
            transform.position + (0.5f * controller.height * -Vector3.up),
            Vector3.up,
            startHeight
        );
        if (checkAbove)
            return;
        ExitCrouch();
    }

    void Crouch()
    {
        Debug.Log(lastSlideTime);
        Debug.Log(Time.time - slideCooldown);
        if (lastSlideTime > Time.time - slideCooldown)
            return;
        Debug.Log("2");
        controller.height = crouchHeight;
        transform.localScale = new Vector3(
            transform.localScale.x,
            crouchHeight,
            transform.localScale.z
        );

        isCrouching = true;
        if (forwardVelocity > runSpeed * 0.8)
        {
            isSliding = true;
            forwardDirection = transform.forward;
            if (isGrounded)
                IncreaseSpeed(slideSpeedIncrease);
            slideTimer = maxSlideTimer;
        }
    }

    void ExitCrouch()
    {
        controller.height = startHeight;
        transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

        isCrouching = false;
        isSliding = false;
        wantsToUncrouch = false;
    }

    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease * Time.deltaTime;
    }

    void SlideMovement()
    {
        move += forwardDirection;
        move = Vector3.ClampMagnitude(move, speed);
    }

    void CalculateForwardVelocity()
    {
        Vector3 currentVelocity = (controller.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = controller.transform.position;
        forwardVelocity = Vector3.Dot(currentVelocity, controller.transform.forward);
    }
}
