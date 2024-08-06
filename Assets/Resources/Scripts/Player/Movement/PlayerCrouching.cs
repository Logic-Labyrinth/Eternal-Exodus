using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    public class PlayerCrouching : MonoBehaviour {
        Rigidbody rb;
        bool      crouching;
        bool      wantsToUncrouch;

        [SerializeField] float crouchSpeed  = 4f;
        [SerializeField] float crouchYScale = 0.5f;

        void Awake() {
            rb = Player.Rigidbody;
            EventForge.Signal.Get("Input.Player.Crouch.Pressed").AddListener(StartCrouch);
            EventForge.Signal.Get("Input.Player.Crouch.Released").AddListener(StopCrouch);
        }

        void StartCrouch() {
            if (crouching) return;
            crouching            = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
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

            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            crouching            = false;
            wantsToUncrouch      = false;
        }
    }
}