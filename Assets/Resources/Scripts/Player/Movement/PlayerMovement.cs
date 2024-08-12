using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerMovement : MonoBehaviour {
        float      moveSpeed;
        float      desiredMoveSpeed;
        Vector2    movementInput;
        RaycastHit slopeHit;
        Rigidbody  rb;

        [SerializeField] float walkSpeed               = 7f;
        [SerializeField] float speedIncreaseMultiplier = 10f;
        [SerializeField] float groundDrag              = 5f;
        [SerializeField] float airMultiplier           = 0.4f;
        [SerializeField] float maxSlopeAngle           = 45f;

#if UNITY_EDITOR
        void OnGUI() {
            GUILayout.Label("Input:    " + movementInput);
            GUILayout.Label("Movement: "       + rb.velocity);
            GUILayout.Label("Grounded: "       + Player.IsGrounded);
        }
#endif

        void Awake() {
            rb            = Player.Rigidbody;
            movementInput = Vector2.zero;
            moveSpeed     = walkSpeed;
            EventForge.Vector2.Get("Input.Player.Movement").AddListener(input => movementInput = input);
        }

        void FixedUpdate() {
            MovePlayer();
        }

        bool OnSlope() {
            if (!Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.3f)) return false;
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        Vector3 GetSlopeMoveDirection(Vector3 direction) {
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }

        void MovePlayer() {
            if (movementInput == Vector2.zero) return;

            Vector3 moveDirection = (Player.Transform.forward * movementInput.y + Player.Transform.right * movementInput.x).normalized;
            if (OnSlope()) {
                var direction = GetSlopeMoveDirection(moveDirection) * moveSpeed / 2f;
                rb.AddForce(direction, ForceMode.Force);
            }
            else {
                float speed = Player.IsGrounded ? moveSpeed : moveSpeed * airMultiplier;
                rb.AddForce(moveDirection * speed, ForceMode.Force);
            }
        }
    }
}