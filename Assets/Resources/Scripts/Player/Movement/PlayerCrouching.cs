using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerCrouching : MonoBehaviour {
        Rigidbody       rb;
        CapsuleCollider collider;
        bool            crouching;
        bool            wantsToUncrouch;
        float           playerHeight;

        [SerializeField] float crouchSpeed  = 4f;
        [SerializeField] float crouchYScale = 0.5f;

        void Awake() {
            rb           = Player.Rigidbody;
            collider     = GetComponent<CapsuleCollider>();
            playerHeight = collider.height;

            EventForge.Signal.Get("Input.Player.Crouch.Pressed").AddListener(StartCrouch);
            EventForge.Signal.Get("Input.Player.Crouch.Released").AddListener(StopCrouch);
        }

        void FixedUpdate() {
            TryUncrouch();
        }

        void StartCrouch() {
            if (crouching) return;
            crouching = true;
            collider.height = crouchYScale;
            rb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        }

        void StopCrouch() {
            if (!crouching) return;
            wantsToUncrouch = true;
        }

        void TryUncrouch() {
            if (!wantsToUncrouch) {
                if (!crouching) wantsToUncrouch = false;
                return;
            }

            Debug.DrawRay(transform.position, Vector3.up * 2);
            bool isObjectAbove = Physics.Raycast(transform.position, Vector3.up, 2);
            if (isObjectAbove) return;

            collider.height = playerHeight;
            crouching       = false;
            wantsToUncrouch = false;
        }
    }
}