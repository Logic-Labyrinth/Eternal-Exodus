using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerMovement : MonoBehaviour {
        float     moveSpeed;
        float     desiredMoveSpeed;
        Vector2   movementInput;
        Transform orientation;
        Rigidbody rb;

        [SerializeField] float walkSpeed               = 7f;
        [SerializeField] float speedIncreaseMultiplier = 10f;
        [SerializeField] float groundDrag              = 5f;
        [SerializeField] float airMultiplier           = 0.4f;

        void Awake() {
            rb            = Player.Rigidbody;
            orientation   = Player.Orientation;
            movementInput = Vector2.zero;
            moveSpeed     = walkSpeed; // testing
            EventForge.Vector2.Get("Input.Player.Movement").AddListener(input => movementInput = input);
        }

        void FixedUpdate() {
            MovePlayer();
            Debug.Log(movementInput);
        }

        void MovePlayer() {
            if (movementInput == Vector2.zero) return;

            Vector2 moveDirection = orientation.forward * movementInput.y + orientation.right * movementInput.x;
            switch (Player.IsGrounded) {
                case true:
                    rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
                    break;
                case false:
                    rb.AddForce(moveDirection.normalized * (moveSpeed * airMultiplier), ForceMode.Force);
                    break;
            }
        }
    }
}