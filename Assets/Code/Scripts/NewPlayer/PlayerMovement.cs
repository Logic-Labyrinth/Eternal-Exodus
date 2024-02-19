using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
  [Header("Movement")]
  [SerializeField] float walkSpeed = 7f;
  [SerializeField] float sprintSpeed = 10f;
  [SerializeField] float slideSpeed;
  [SerializeField] float groundDrag = 5f;
  [SerializeField] float speedIncreaseMultiplier = 10f;
  [SerializeField] float slopeIncreaseMultiplier = 2.5f;
  float moveSpeed;
  float desiredMoveSpeed;
  float lastDesiredMoveSpeed;
  bool isSliding;

  [Header("Jumping")]
  [SerializeField] float jumpForce = 12f;
  [SerializeField] float jumpCooldown = 0.25f;
  [SerializeField] float airMultiplier = 0.4f;
  bool readyToJump = true;

  [Header("Crouching")]
  public float crouchSpeed = 4f;
  [SerializeField] float crouchYScale = 0.5f;
  float startYScale;

  [Header("Ground Check")]
  [SerializeField] LayerMask groundLayer;
  bool isGrounded;

  [Header("Slope Handling")]
  [SerializeField] float maxSlopeAngle = 45f;
  RaycastHit slopeHit;
  bool exitingSlope;

  float horizontalInput;
  float verticalInput;

  Transform orientation;
  Vector3 moveDirection;
  Vector3 flatVelocity;
  Rigidbody rb;
  MovementState state;

  public enum MovementState {
    Walking,
    Sprinting,
    Crouching,
    Sliding,
    Air
  }

  void OnGUI() {
    GUILayout.Label($"State: {state}");
    GUILayout.TextArea($"Grounded: {isGrounded}");
    GUILayout.TextArea($"Ready to jump: {readyToJump}");
    GUILayout.TextArea($"Move direction: {moveDirection}");
    GUILayout.TextArea($"Current speed: {flatVelocity.magnitude}");
  }

  private void Start() {
    rb = GetComponent<Rigidbody>();
    orientation = transform.Find("Orientation");
    rb.freezeRotation = true;
    moveSpeed = walkSpeed;
    startYScale = transform.localScale.y;
  }

  private void Update() {
    GroundCheck();
    HandleInput();
    SpeedControl();
    HandleState();

    rb.drag = isGrounded ? groundDrag : 0;
  }

  private void FixedUpdate() {
    MovePlayer();
  }

  private void HandleInput() {
    horizontalInput = Input.GetAxisRaw("Horizontal");
    verticalInput = Input.GetAxisRaw("Vertical");

    if (Input.GetKey(KeyCode.Space) && readyToJump && isGrounded) {
      readyToJump = false;
      Jump();
      Invoke(nameof(ResetJump), jumpCooldown);
    }

    if (Input.GetKeyDown(KeyCode.LeftControl)) {
      transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
      rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    if (Input.GetKeyUp(KeyCode.LeftControl)) {
      transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }
  }

  void HandleState() {
    if (isSliding) {
      state = MovementState.Sliding;
      if(OnSlope() && rb.velocity.y < 0.1f) {
        desiredMoveSpeed = slideSpeed;
      } else {
        desiredMoveSpeed = sprintSpeed;
      }
    } else if (Input.GetKey(KeyCode.LeftControl) && rb.velocity.magnitude <= crouchSpeed * 1.1f) {
      state = MovementState.Crouching;
      desiredMoveSpeed = crouchSpeed;
    } else if (isGrounded && Input.GetKey(KeyCode.LeftShift)) {
      state = MovementState.Sprinting;
      desiredMoveSpeed = sprintSpeed;
    } else if (isGrounded) {
      state = MovementState.Walking;
      desiredMoveSpeed = walkSpeed;
    } else {
      state = MovementState.Air;
    }

    if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0) {
      StopCoroutine(LerpMoveSpeed());
      StartCoroutine(LerpMoveSpeed());
    } else {
      moveSpeed = desiredMoveSpeed;
    }

    lastDesiredMoveSpeed = desiredMoveSpeed;
  }

  IEnumerator LerpMoveSpeed(){
    float time = 0;
    float difference = Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed);
    float startValue = moveSpeed;

    while (time < difference) {
      moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

      if(OnSlope()){
        float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
        float slopeAngleIncrease = 1 + (slopeAngle / 90f);
        time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
      } else {
        time += Time.deltaTime * speedIncreaseMultiplier;
      }

      yield return null;
    }

    moveSpeed = desiredMoveSpeed;
  }

  private void MovePlayer() {
    moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
    moveDirection.y = 0;

    if (OnSlope() && !exitingSlope) {
      rb.AddForce(20f * moveSpeed * GetSlopeMoveDirection(moveDirection), ForceMode.Force);
      if (rb.velocity.y > 0) {
        rb.AddForce(Vector3.down * 80f, ForceMode.Force);
      }
    }

    if (isGrounded) {
      rb.AddForce(10f * moveSpeed * moveDirection.normalized, ForceMode.Force);
    } else
        if (!isGrounded) {
      rb.AddForce(10f * moveSpeed * airMultiplier * moveDirection.normalized, ForceMode.Force);
    }

    rb.useGravity = !OnSlope();
  }

  private void GroundCheck() {
    isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayer);
  }

  private void SpeedControl() {
    if (OnSlope() && !exitingSlope) {
      if (rb.velocity.magnitude > moveSpeed) {
        // Vector3 limitedVelocity = rb.velocity.normalized * moveSpeed;
        rb.velocity = rb.velocity.normalized * moveSpeed;
      }
    } else {
      flatVelocity = new(rb.velocity.x, 0, rb.velocity.z);

      if (flatVelocity.magnitude > moveSpeed) {
        Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
        rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
      }
    }
  }

  public void Jump() {
    exitingSlope = true;
    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
  }

  private void ResetJump() {
    readyToJump = true;
    exitingSlope = false;
  }

  public bool OnSlope() {
    if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 0.3f)) {
      float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
      return angle < maxSlopeAngle && angle != 0;
    }

    return false;
  }

  public Vector3 GetSlopeMoveDirection(Vector3 direction) {
    return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
  }
}
