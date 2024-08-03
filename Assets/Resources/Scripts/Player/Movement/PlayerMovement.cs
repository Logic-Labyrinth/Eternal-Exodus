using System;
using System.Collections;
using LexUtils.Events;
using TEE.VFX;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerMovement : MonoBehaviour {
        bool disableMovementInput;

        [Header("Movement")] float moveSpeed;
        float                      desiredMoveSpeed;
        float                      lastDesiredMoveSpeed;
        [SerializeField] float     walkSpeed               = 7f;
        [SerializeField] float     sprintSpeed             = 10f;
        [SerializeField] float     slideSpeed              = 30f;
        [SerializeField] float     speedIncreaseMultiplier = 10f;
        [SerializeField] float     slopeIncreaseMultiplier = 2.5f;
        [SerializeField] float     groundDrag              = 5f;

        [Header("Jumping")] [SerializeField] float jumpForce                 = 12f;
        [SerializeField]                     float jumpCooldown              = 0.25f;
        [SerializeField]                     float airMultiplier             = 0.4f;
        [SerializeField]                     float jumpFallGravityMultiplier = 2.5f;
        [SerializeField]                     float swordJumpMultiplier       = 1.2f;
        bool                                       canJump                   = true;

        [Header("Crouching")] public float crouchSpeed  = 4f;
        [SerializeField]             float crouchYScale = 0.5f;
        float                              startYScale;

        [Header("Ground Check")] [SerializeField]
        LayerMask groundMask;

        const  float PlayerHeight = 2f;
        public bool  isGrounded;

        [Header("Slope Handling")] [SerializeField]
        float maxSlopeAngle = 45f;

        RaycastHit slopeHit;
        bool       exitingSlope;

        float horizontalInput;
        float verticalInput;

        public           Transform orientation;
        [SerializeField] Transform playerObj;
        Vector3                    moveDirection;
        Vector3                    flatVelocity;
        public Rigidbody           rb;
        MovementState              state;
        public MovementState       lastState;

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
        bool        keepMomentum;

        [Header("VFX")] bool           lastFrameGrounded;
        float                          lastFrameVerticalVelocity;
        [SerializeField] GameObject    landingVFXPrefab;
        [SerializeField] DustTrailVFX  dustTrailVFX;
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

        void Start() {
            rb                        = GetComponent<Rigidbody>();
            rb.freezeRotation         = true;
            startYScale               = playerObj.localScale.y;
            wantsToUncrouch           = false;
            lastFrameVerticalVelocity = 0;
            
            EventForge.Vector2.Get("Input.Player.Movement").AddListener(MovePlayer);
        }

        void Update() {
            HandleInput();
            HandleGravity();
            HandleVFX();
        }

        void FixedUpdate() {
            GroundCheck();
            // MovePlayer();
            SpeedControl();
            StateHandler();

            if (dashing) rb.drag                                                                          = 0;
            else if (state is MovementState.Walk or MovementState.Sprint or MovementState.Crouch) rb.drag = groundDrag;
            else rb.drag                                                                                  = 0;
        }

        void HandleGravity() {
            if (rb.velocity.y < 0) rb.velocity += (jumpFallGravityMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }

        void HandleVFX() {
            dustTrailVFX.SetSpeed(rb.velocity.magnitude);
            dustTrailVFX.SetCanPlay(isGrounded);
            speedLinesVFX.SetSpeed(rb.velocity.magnitude);
        }

        void GroundCheck() {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f, groundMask);
            Debug.DrawRay(transform.position, Vector3.down * 1.2f, Color.magenta);

            if (!lastFrameGrounded && isGrounded) {
                Landed();
            }

            lastFrameGrounded         = isGrounded;
            lastFrameVerticalVelocity = rb.velocity.y;
        }

        void Landed() {
            var landingVFX = Instantiate(landingVFXPrefab, new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z), Quaternion.identity).GetComponent<LandingVFX>();
            landingVFX.Play(Math.Abs(lastFrameVerticalVelocity));
        }

        void HandleInput() {
            if (disableMovementInput) return;

            // horizontalInput = Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Horizontal Controller");
            // verticalInput   = Input.GetAxisRaw("Vertical")   + Input.GetAxisRaw("Vertical Controller");
            //
            // if (Input.GetButtonDown("Jump") && canJump && isGrounded) {
            //     // canJump = false;
            //     Jump();
            //     StartCoroutine(ResetJump());
            // }

            TryUncrouch();
        }

        public void StartCrouch() {
            crouching            = true;
            playerObj.localScale = new Vector3(playerObj.localScale.x, crouchYScale, playerObj.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        void StopCrouch() {
            if (!crouching) return;

            playerObj.localScale = new Vector3(playerObj.localScale.x, 1, playerObj.localScale.z);
            crouching            = false;
            wantsToUncrouch      = false;
        }

        void TryUncrouch() {
            if (!wantsToUncrouch) {
                if (!crouching) wantsToUncrouch = false;
                return;
            }

            Debug.DrawRay(transform.position, Vector3.up * PlayerHeight);
            bool checkAbove = Physics.Raycast(transform.position, Vector3.up, PlayerHeight, groundMask);
            if (checkAbove) return;
            StopCrouch();
        }

        void StateHandler() {
            if (dashing) {
                desiredMoveSpeed = slideSpeed;
            }
            else if (sliding) {
                state = MovementState.Slide;
                if (OnSlope() && rb.velocity.y < 0.1f) desiredMoveSpeed = slideSpeed;
                else desiredMoveSpeed                                   = crouchSpeed;
            }
            else if (crouching) {
                state            = MovementState.Crouch;
                desiredMoveSpeed = crouchSpeed;
            }
            else
                switch (isGrounded) {
                    // case true when Input.GetButton("Crouch"):
                    //     state            = MovementState.Sprint;
                    //     desiredMoveSpeed = sprintSpeed;
                    //     break;
                    case true:
                        state            = MovementState.Walk;
                        desiredMoveSpeed = walkSpeed;
                        break;
                    default: {
                        state            = MovementState.Glide;
                        desiredMoveSpeed = desiredMoveSpeed < sprintSpeed ? walkSpeed : sprintSpeed;
                        break;
                    }
                }

            if (lastState is MovementState.Slide or MovementState.Walk) keepMomentum = true;
            bool speedChanged                                                        = !Mathf.Approximately(desiredMoveSpeed, lastDesiredMoveSpeed);
            if (speedChanged) {
                if (keepMomentum) {
                    StopAllCoroutines();
                    StartCoroutine(SmoothlyLerpMoveSpeed());
                }
                else {
                    StopAllCoroutines();
                    moveSpeed = desiredMoveSpeed;
                }
            }

            lastDesiredMoveSpeed = desiredMoveSpeed;
            lastState            = state;
        }

        IEnumerator SmoothlyLerpMoveSpeed() {
            float time       = 0;
            float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
            float startValue = moveSpeed;

            while (time < difference) {
                moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

                if (OnSlope()) {
                    float slopeAngle         = Vector3.Angle(Vector3.up, slopeHit.normal);
                    float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                    time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
                }
                else
                    time += Time.deltaTime * speedIncreaseMultiplier;

                yield return null;
            }

            moveSpeed    = desiredMoveSpeed;
            keepMomentum = false;
        }

        void MovePlayer(Vector2 input) {
            if (disableMovementInput) return;
            if (dashing) return;

            // calculate movement direction
            // moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            moveDirection = orientation.forward * input.y + orientation.right * input.x;

            // on slope
            if (OnSlope() && !exitingSlope) {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * (moveSpeed * 20f), ForceMode.Force);
                if (rb.velocity.y > 0) rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else
                switch (isGrounded) {
                    case true:
                        // on ground
                        rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
                        break;
                    case false:
                        // in air
                        rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
                        break;
                }

            // turn gravity off while on slope
            rb.useGravity = !OnSlope();
        }

        void SpeedControl() {
            // limiting speed on slope
            if (OnSlope() && !exitingSlope) {
                if (rb.velocity.magnitude > moveSpeed) rb.velocity = rb.velocity.normalized * moveSpeed;
            }

            // limiting speed on ground or in air
            else {
                Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                // limit velocity if needed
                if (!(flatVel.magnitude > moveSpeed)) return;
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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

        IEnumerator ResetJump() {
            yield return new WaitForSeconds(jumpCooldown);
            canJump      = true;
            exitingSlope = false;
        }

        public bool OnSlope() {
            if (!Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.3f)) return false;
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
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
}