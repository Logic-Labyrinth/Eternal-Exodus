using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Movement {
    public class GroundCheck : MonoBehaviour {
        [SerializeField] LayerMask groundLayer;
        SphereCollider             sphereCollider;
        bool                       previousFrameAirborne;

        void Start() {
            sphereCollider = GetComponent<SphereCollider>();
        }

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
            if (previousFrameAirborne) EventForge.Signal.Get("Player.Landed").Invoke();
            Player.IsGrounded     = true;
            previousFrameAirborne = false;
        }

        void OnTriggerExit(Collider other) {
            if (other.gameObject.layer != LayerMask.NameToLayer("Ground")) return;
            var hit = Physics.CheckSphere(transform.position, sphereCollider.radius, groundLayer, QueryTriggerInteraction.Ignore);
            if (hit) return;
            Player.IsGrounded = false;
            EventForge.Signal.Get("Player.Liftoff").Invoke();
            previousFrameAirborne = true;
        }
    }
}