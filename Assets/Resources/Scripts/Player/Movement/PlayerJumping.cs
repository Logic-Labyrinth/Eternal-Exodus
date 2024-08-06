using System.Collections;
using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerJumping : MonoBehaviour {
        Rigidbody              rb;
        Vector3                jumpPower;
        bool                   canJump                   = true;
        [SerializeField] float jumpForce                 = 12f;
        [SerializeField] float jumpCooldown              = 0.25f;
        [SerializeField] float jumpFallGravityMultiplier = 2.5f;
        [SerializeField] float swordJumpMultiplier       = 1.2f;

        void Awake() {
            rb        = Player.Rigidbody;
            jumpPower = transform.up * jumpForce;

            EventForge.Signal.Get("Input.Player.Jump").AddListener(Jump);
            EventForge.Float.Get("Input.Player.Jump").AddListener(Jump);
        }

        void FixedUpdate() {
            // Increase gravity when falling
            if (rb.velocity.y < 0) {
                rb.velocity += (jumpFallGravityMultiplier - 1) * Physics.gravity.y * Time.fixedDeltaTime * Vector3.up;
            }
        }

        void Jump() {
            if (!canJump) return;
            canJump = false;
            StartCoroutine(ResetJump());
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(jumpPower, ForceMode.Impulse);
        }

        void Jump(float force) {
            if (!canJump) return;
            canJump = false;
            StartCoroutine(ResetJump());
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * force, ForceMode.Impulse);
        }

        IEnumerator ResetJump() {
            yield return new WaitForSeconds(jumpCooldown);
            canJump = true;
        }
    }
}