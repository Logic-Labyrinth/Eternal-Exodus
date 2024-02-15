using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {
    [BoxGroup("Settings")]
    [TitleGroup("Settings/Speeds")]
    [SerializeField]
    float runSpeed;
    [TitleGroup("Settings/Speeds")]
    [SerializeField]
    float airSpeed;
    [TitleGroup("Settings/Speeds")]
    [SerializeField]
    float crouchSpeed;
    [TitleGroup("Settings/Speeds")]
    [SerializeField, Range(0f, 1f)]
    float airControl = 1f;

    [TitleGroup("Settings/Slide")]
    [SerializeField]
    float slideCooldown = 2f;
    [TitleGroup("Settings/Slide")]
    [SerializeField]
    float maxSlideTimer = 2f;
    [TitleGroup("Settings/Slide")]
    [SerializeField]
    float slideSpeedIncrease = 2.5f;
    [TitleGroup("Settings/Slide")]
    [SerializeField]
    float slideSpeedDecrease = 3f;

    [TitleGroup("Settings/Jump")]
    [SerializeField]
    float jumpHeight;
    [TitleGroup("Settings/Jump")]
    [SerializeField]
    int jumpCharges = 2;

    [TitleGroup("Settings/Miscellaneous")]
    [SerializeField]
    float fastFOV;
    [TitleGroup("Settings/Miscellaneous")]
    [SerializeField]
    float normalGravity;
    [TitleGroup("Settings/Miscellaneous")]
    [SerializeField]
    float crouchHeight = 0.5f;

    [Space(30)]
    [Title("Extra")]
    [SerializeField]
    Camera cam;

    CharacterController controller;
    Vector3 move, input, velocityY, forwardDirection, lastPosition;
    float speed, gravity, startHeight, slideTimer, normalFOV;
    float forwardVelocity = 0f;
    float lastSlideTime;
    int jumpChargesLeft;
    bool isGrounded, isCrouching, isSliding, wantsToUncrouch;
    bool useNormalGravity;

    private void OnGUI() {
        GUILayout.TextArea("Wants to uncrouch: " + wantsToUncrouch);
        GUILayout.TextArea("Player pos: " + controller.transform.position);
        GUILayout.TextArea("Fwd vector: " + forwardVelocity);
        GUILayout.TextArea("FOV: " + cam.fieldOfView);
        GUILayout.TextArea("Jump charges: " + (jumpChargesLeft - 1));
        GUILayout.TextArea("Height: " + controller.height);
        GUILayout.TextArea("Start height: " + startHeight);
    }

    void Start() {
        controller = GetComponent<CharacterController>();
        startHeight = transform.localScale.y;
        normalFOV = cam.fieldOfView;
        lastPosition = controller.transform.position;
        useNormalGravity = true;
        wantsToUncrouch = false;
        lastSlideTime = Time.time;
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

        if (Input.GetButtonDown("Jump") && jumpChargesLeft > 1) Jump();
        if (Input.GetButtonDown("Crouch")) Crouch();
        if (Input.GetButtonUp("Crouch")) wantsToUncrouch = true;
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

        move = Vector3.ClampMagnitude(move * airControl, speed);
    }

    void ApplyGravity() {
        if (useNormalGravity) gravity = normalGravity;
        velocityY.y -= gravity * Time.deltaTime;
        controller.Move(velocityY * Time.deltaTime);
    }

    void CheckGround() {
        isGrounded = Physics.Raycast(transform.position + (0.5f * controller.height * -Vector3.up), -Vector3.up, 0.2f);
        if (isGrounded) jumpChargesLeft = jumpCharges;
    }

    void Jump() {
        velocityY.y = Mathf.Sqrt(jumpHeight * 2 * normalGravity);
        jumpChargesLeft--;
        // if (isCrouching) wantsToUncrouch = true;
    }

    void HandleMovement() {
        TryUncrouch();
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

    void TryUncrouch() {
        if (!isCrouching) return;
        if (!wantsToUncrouch) return;
        if (wantsToUncrouch && controller.height == startHeight * 2) { wantsToUncrouch = false; isCrouching = false; }
        bool checkAbove = Physics.Raycast(transform.position + (0.5f * controller.height * -Vector3.up), Vector3.up, startHeight * 2);
        if (checkAbove) return;
        ExitCrouch();
    }

    void Crouch() {
        if (lastSlideTime > Time.time - slideCooldown) return;
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
        wantsToUncrouch = false;
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
