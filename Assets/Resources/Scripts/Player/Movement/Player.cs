using LexUtils.Events;
using TEE.Input;
using UnityEngine;

namespace TEE.Player.Movement {
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour {
        [SerializeField] Transform orientation;
        [SerializeField] Transform groundCheckOrigin;
        public static    Rigidbody Rigidbody  { get; private set; }
        public static    bool      IsGrounded { get; private set; }
        public static    Transform Orientation;

        bool previousFrameGrounded;

        void Awake() {
            Rigidbody                = GetComponent<Rigidbody>();
            Rigidbody.freezeRotation = true;
            Orientation              = orientation;
        }

        void Start() {
            InputManager.SetCursorEnabled(false);
        }

        void FixedUpdate() {
            CheckGround();
        }

        void CheckGround() {
            bool hitGround = Physics.SphereCast(groundCheckOrigin.position, 0.5f, Vector3.down, out var hit, 0.1f, ~0, QueryTriggerInteraction.Ignore);

            if (hitGround) {
                if (previousFrameGrounded) return;

                // Landed
                EventForge.Signal.Get("Player.Landed").Invoke();
                previousFrameGrounded = true;
                IsGrounded            = true;
                return;
            }

            if (!previousFrameGrounded) return;
            // Left ground
            EventForge.Signal.Get("Player.LeftGround").Invoke();
            previousFrameGrounded = false;
            IsGrounded            = false;
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(groundCheckOrigin.position, 0.5f);
        }
    }
}