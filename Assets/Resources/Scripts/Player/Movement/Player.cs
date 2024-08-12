using TEE.Input;
using UnityEngine;

namespace TEE.Player.Movement {
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour {
        [SerializeField] Transform groundCheckOrigin;
        [SerializeField] LayerMask groundLayer;
        public static    Rigidbody Rigidbody  { get; private set; }
        public static    bool      IsGrounded;
        public static    Transform Transform;

        bool previousFrameGrounded;

        void Awake() {
            Rigidbody                = GetComponent<Rigidbody>();
            Rigidbody.freezeRotation = true;
            Transform                = transform;
        }

        void Start() {
            InputManager.SetCursorEnabled(false);
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(groundCheckOrigin.position, 0.5f);
        }
    }
}