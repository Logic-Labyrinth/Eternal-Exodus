using System;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour {
  [BoxGroup("Settings")]
  [TitleGroup("Settings/Speeds"), SerializeField] float runSpeed;
  [TitleGroup("Settings/Speeds"), SerializeField] float airSpeed;
  [TitleGroup("Settings/Speeds"), SerializeField] float crouchSpeed;
  [TitleGroup("Settings/Speeds"), SerializeField, Range(0f, 1f)] float airControl = 1f;

  [TitleGroup("Settings/Slide"), SerializeField] float slideCooldown = 2f;
  [TitleGroup("Settings/Slide"), SerializeField] float maxSlideTimer = 2f;
  [TitleGroup("Settings/Slide"), SerializeField] float slideSpeedIncrease = 2.5f;
  [TitleGroup("Settings/Slide"), SerializeField] float slideSpeedDecrease = 3f;

  [TitleGroup("Settings/Jump"), SerializeField] float jumpHeight;
  [TitleGroup("Settings/Jump"), SerializeField] int jumpCharges = 2;

  [TitleGroup("Settings/Miscellaneous"), SerializeField] float fastFOV;
  [TitleGroup("Settings/Miscellaneous"), SerializeField] float normalGravity;
  [TitleGroup("Settings/Miscellaneous"), SerializeField] float crouchHeight = 0.5f;

  [Space(30)]

  [Title("Extra"), SerializeField] Camera cam;

  CharacterController controller;
  Vector3 move, input, velocityY, forwardDirection, lastPosition;
  float speed, startHeight, slideTimer, normalFOV;
  float forwardVelocity = 0f;
  float lastSlideTime;
  int jumpChargesLeft;
  bool isGrounded, isCrouching, isSliding, wantsToUncrouch;

  private void OnGUI() {
    GUILayout.TextArea("Wants to uncrouch: " + wantsToUncrouch);
    GUILayout.TextArea("Crouching: " + isCrouching);
    GUILayout.TextArea("Player pos: " + controller.transform.position);
    GUILayout.TextArea("Fwd vector: " + forwardVelocity);
    GUILayout.TextArea("FOV: " + cam.fieldOfView);
    GUILayout.TextArea("Jump charges: " + (jumpChargesLeft - 1));
    GUILayout.TextArea("Height: " + controller.height);
    GUILayout.TextArea("Start height: " + startHeight);
  }

  void Start() {
    controller = GetComponent<CharacterController>();
    startHeight = controller.height;
    normalFOV = cam.fieldOfView;
    lastPosition = controller.transform.position;
    wantsToUncrouch = false;
    lastSlideTime = Mathf.NegativeInfinity;
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

  // HandleInput function to process input and perform corresponding actions.
  void HandleInput() {
    input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    input = transform.TransformDirection(input);
    input = Vector3.ClampMagnitude(input, 1f);

    if (Input.GetButtonDown("Jump") && jumpChargesLeft > 1) Jump();
    if (Input.GetButtonDown("Crouch")) Crouch();
    if (Input.GetButtonUp("Crouch")) wantsToUncrouch = true;
  }

  // A function to handle camera effects.
  void CameraEffects() {
    float targetFOV = Mathf.Lerp(normalFOV, fastFOV, forwardVelocity / runSpeed);
    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * 10f);
  }

  // Function for handling ground movement, adjusting speed based on crouching state and input, and clamping the movement vector.
  void GroundMovement() {
    speed = isCrouching ? crouchSpeed : runSpeed;
    move.x = input.x == 0 ? 0 : input.x * speed;
    move.z = input.z == 0 ? 0 : input.z * speed;

    move = Vector3.ClampMagnitude(move, speed);
  }

  // A function to handle air movement, setting the speed based on airSpeed, and calculating the movement in the x and z directions using the input and speed. The resulting movement is then clamped to the specified airControl and speed.
  void AirMovement() {
    speed = airSpeed;
    move.x = input.x == 0 ? 0 : input.x * speed;
    move.z = input.z == 0 ? 0 : input.z * speed;

    move = Vector3.ClampMagnitude(move * airControl, speed);
  }

  // ApplyGravity function applies a gravitational force to the object's velocity in the y-axis and moves the controller accordingly.
  void ApplyGravity() {
    velocityY.y -= normalGravity * Time.deltaTime;
    controller.Move(velocityY * Time.deltaTime);
  }

  // CheckGround function performs a raycast to check if the object is grounded, and updates the jump charges left if the object is grounded.
  void CheckGround() {
    Vector3 bottomPos = transform.position + (0.5f * controller.height * -Vector3.up);
    isGrounded = Physics.Raycast(bottomPos, -Vector3.up, 0.2f);
    Debug.DrawRay(bottomPos, -Vector3.up);

    if (isGrounded) jumpChargesLeft = jumpCharges;
  }

  // A function to perform a jump action, adjusting the Y velocity and decrementing the remaining jump charges.
  void Jump() {
    velocityY.y = Mathf.Sqrt(jumpHeight * 2 * normalGravity);
    jumpChargesLeft--;
    // if (isCrouching) wantsToUncrouch = true;
  }

  // HandleMovement function handles the movement of the character. It tries to uncrouch, and then based on the character's state, it either performs ground movement, air movement, or slide movement. It also decreases the slide speed and checks if the sliding should stop.
  void HandleMovement() {
    TryUncrouch();
    if (isGrounded && !isSliding) GroundMovement();
    else if (!isGrounded) AirMovement();
    else if (isSliding) {
      SlideMovement();
      DecreaseSpeed(slideSpeedDecrease);
      slideTimer -= Time.deltaTime;
      if (slideTimer <= 0) isSliding = false;
    }
  }

  // This function attempts to uncrouch the character, checking if it is possible to do so without colliding with any obstacles. It does not take any parameters and does not return any value.
  void TryUncrouch() {
    if (!wantsToUncrouch) {
      if (!isCrouching) wantsToUncrouch = false;
      return;
    }

    Vector3 startPos = transform.position + (0.5f * controller.height * Vector3.up);
    Debug.DrawRay(startPos, Vector3.up * (startHeight - controller.height));
    bool checkAbove = Physics.Raycast(startPos, Vector3.up, startHeight - controller.height);
    if (checkAbove) return;
    ExitCrouch();
  }

  // A function to crouch, adjusting the player's height and triggering sliding if moving at a high velocity.
  void Crouch() {
    if (lastSlideTime > Time.time - slideCooldown) return;
    controller.height = crouchHeight;
    transform.localScale = new Vector3(transform.localScale.x, crouchHeight * 0.5f, transform.localScale.z);

    isCrouching = true;
    if (forwardVelocity > runSpeed * 0.8) {
      isSliding = true;
      forwardDirection = transform.forward;
      if (isGrounded) IncreaseSpeed(slideSpeedIncrease);
      slideTimer = maxSlideTimer;
    }
  }

  // Function to exit the crouching state, restoring the original height and scale while resetting crouching and sliding flags.
  void ExitCrouch() {
    controller.height = startHeight;
    transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);

    isCrouching = false;
    isSliding = false;
    wantsToUncrouch = false;
  }

  // Increases the speed of the object by the specified amount.
  void IncreaseSpeed(float speedIncrease) {
    speed += speedIncrease;
  }

  void DecreaseSpeed(float speedDecrease) {
    speed -= speedDecrease * Time.deltaTime;
  }

  // A function to handle the movement of an object in the game world.
  void SlideMovement() {
    move += forwardDirection;
    move = Vector3.ClampMagnitude(move, speed);
  }

  // Calculate the forward velocity based on the current velocity, last position, and time difference.
  void CalculateForwardVelocity() {
    Vector3 currentVelocity = (controller.transform.position - lastPosition) / Time.deltaTime;
    lastPosition = controller.transform.position;
    forwardVelocity = Vector3.Dot(currentVelocity, controller.transform.forward);
  }
}
