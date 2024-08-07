using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerMovement : MonoBehaviour {
        float     moveSpeed;
        float     desiredMoveSpeed;
        Vector2   movementInput;
        Rigidbody rb;

        [SerializeField] float walkSpeed               = 7f;
        [SerializeField] float speedIncreaseMultiplier = 10f;
        [SerializeField] float groundDrag              = 5f;
        [SerializeField] float airMultiplier           = 0.4f;

#if UNITY_EDITOR
        void OnGUI() {
            GUILayout.Label("Movement Input: " + movementInput);
            GUILayout.Label("Movement: "       + (Player.Transform.forward * movementInput.y + Player.Transform.right * movementInput.x) * moveSpeed);
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

        void MovePlayer() {
            if (movementInput == Vector2.zero) return;

            Vector3 moveDirection = Player.Transform.forward * movementInput.y + Player.Transform.right * movementInput.x;

            float speed = Player.IsGrounded ? moveSpeed : moveSpeed * airMultiplier;
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
        }
    }
}