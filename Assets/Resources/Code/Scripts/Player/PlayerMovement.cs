using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    bool disableMovementInput = false;

    [Header("Movement")]
    float moveSpeed;
    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    [SerializeField] float walkSpeed = 7f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float slideSpeed = 30f;
    [SerializeField] float speedIncreaseMultiplier = 10f;
    [SerializeField] float slopeIncreaseMultiplier = 2.5f;
    [SerializeField] float groundDrag = 5f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpCooldown = 0.25f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float jumpFallGravityMultiplier = 2.5f;
    [SerializeField] float swordJumpMultiplier = 1.2f;
    bool canJump = true;

    [Header("Crouching")]
    public float crouchSpeed = 4f;
    [SerializeField] float crouchYScale = 0.5f;
    float startYScale;

    [Header("Ground Check")]
    [SerializeField] LayerMask groundMask;
    readonly float playerHeight = 2f;
    bool isGrounded;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle = 45f;
    RaycastHit slopeHit;
    bool exitingSlope;

    float horizontalInput;
    float verticalInput;

    public Transform orientation;
    [SerializeField] Transform playerObj;
    Vector3 moveDirection;
    Vector3 flatVelocity;
    public Rigidbody rb;
    MovementState state;
    public MovementState lastState;
    public enum MovementState {
        Walk,
        Sprint,
        Crouch,
        Slide,
        Glide
    }

    public bool sliding;
    public bool crouching;
    public bool dashing;
    public bool wantsToUncrouch;
    bool keepMomentum;

    [Header("VFX")]
    bool lastFrameGrounded;
    float lastFrameVerticalVelocity;
    [SerializeField] GameObject landingVFXPrefab;
    [SerializeField] DustTrailVFX dustTrailVFX;
    [SerializeField] SpeedLinesVFX speedLinesVFX;

    // void OnGUI() {
    // GUILayout.TextArea($"State: {state}");
    // GUILayout.TextArea($"Grounded: {isGrounded}");
    // GUILayout.TextArea($"Wants to uncrouch: {wantsToUncrouch}");
    // GUILayout.TextArea($"Sliding: {sliding}");
    // GUILayout.TextArea($"Crouching: {crouching}");
    // GUILayout.TextArea($"Dashing: {dashing}");
    // GUILayout.TextArea($"Player height: {playerHeight}");
    // GUILayout.TextArea($"Player scale: {playerObj.localScale}");
    // GUILayout.TextArea($"Ready to jump: {canJump}");
    // GUILayout.TextArea($"Move direction: {moveDirection}");
    // GUILayout.TextArea($"Current speed: {rb.velocity.magnitude}");
    // GUILayout.TextArea($"Desired speed: {desiredMoveSpeed}");
    // }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = playerObj.localScale.y;
        wantsToUncrouch = false;
        lastFrameVerticalVelocity = 0;
    }

    private void Update() {
        HandleInput();
        HandleGravity();
        HandleVFX();
    }

    private void FixedUpdate() {
        GroundCheck();
        MovePlayer();
        SpeedControl();
        StateHandler();

        if (dashing) rb.drag = 0;
        else if (state == MovementState.Walk || state == MovementState.Sprint || state == MovementState.Crouch) rb.drag = groundDrag;
        else rb.drag = 0;
    }

    void HandleGravity() {
        if (rb.velocity.y < 0) rb.velocity += (jumpFallGravityMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
    }

    void HandleVFX() {
        dustTrailVFX.SetSpeed(rb.velocity.magnitude);
        dustTrailVFX.SetCanPlay(isGrounded);
        speedLinesVFX.SetSpeed(rb.velocity.magnitude);
    }

    private void GroundCheck() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * 1.2f, Color.magenta);

        if (!lastFrameGrounded && isGrounded) {
            Landed();
        }

        lastFrameGrounded = isGrounded;
        lastFrameVerticalVelocity = rb.velocity.y;
    }

    private void Landed() {
        LandingVFX landingVFX = Instantiate(landingVFXPrefab, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), Quaternion.identity).GetComponent<LandingVFX>();
        landingVFX.Play(Math.Abs(lastFrameVerticalVelocity));
    }

    private void HandleInput() {
        if (disableMovementInput) return;

        horizontalInput = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal Controller");
        verticalInput = Input.GetAxisRaw("Vertical") + Input.GetAxisRaw("Vertical Controller");

        if (Input.GetButton("Jump") && canJump && isGrounded) {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        TryUncrouch();
    }

    public void StartCrouch() {
        crouching = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, crouchYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    void StopCrouch() {
        if (!crouching) return;

        playerObj.localScale = new Vector3(playerObj.localScale.x, 1, playerObj.localScale.z);
        crouching = false;
        wantsToUncrouch = false;
    }

    void TryUncrouch() {
        if (!wantsToUncrouch) {
            if (!crouching) wantsToUncrouch = false;
            return;
        }

        Debug.DrawRay(transform.position, Vector3.up * playerHeight);
        bool checkAbove = Physics.Raycast(transform.position, Vector3.up, playerHeight, groundMask);
        if (checkAbove) return;
        StopCrouch();
    }

    private void StateHandler() {
        if (dashing) {
            desiredMoveSpeed = slideSpeed;

        } else if (sliding) {
            state = MovementState.Slide;
            if (OnSlope() && rb.velocity.y < 0.1f) desiredMoveSpeed = slideSpeed;
            else desiredMoveSpeed = crouchSpeed;

        } else if (crouching) {
            state = MovementState.Crouch;
            desiredMoveSpeed = crouchSpeed;

        } else if (isGrounded && Input.GetButton("Crouch")) {
            state = MovementState.Sprint;
            desiredMoveSpeed = sprintSpeed;

        } else if (isGrounded) {
            state = MovementState.Walk;
            desiredMoveSpeed = walkSpeed;

        } else {
            state = MovementState.Glide;
            if (desiredMoveSpeed < sprintSpeed) desiredMoveSpeed = walkSpeed;
            else desiredMoveSpeed = sprintSpeed;
        }

        if (lastState == MovementState.Slide || lastState == MovementState.Walk) keepMomentum = true;
        bool speedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (speedChanged) {
            if (keepMomentum) {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            } else {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private IEnumerator SmoothlyLerpMoveSpeed() {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference) {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope()) {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            } else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        keepMomentum = false;
    }

    private void MovePlayer() {
        if (dashing) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope) {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        // on ground
        else if (isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // in air
        else if (!isGrounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl() {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope) {
            if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed) {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void Jump() {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void SwordJump() {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce * swordJumpMultiplier, ForceMode.Impulse);
    }

    private void ResetJump() {
        canJump = true;
        exitingSlope = false;
    }

    public bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction) {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public void DisableMovementInput() {
        disableMovementInput = true;
    }

    public void EnableMovementInput() {
        disableMovementInput = false;
    }
}
