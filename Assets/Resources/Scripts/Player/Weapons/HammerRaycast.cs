using UnityEngine;

namespace TEE.Player.Weapons {
    public class HammerRaycast : MonoBehaviour {
        [SerializeField] float     distance        = 2f;
        [SerializeField] float     verticalPadding = 0.01f;
        [SerializeField] LayerMask layer;

        public Vector3 Raycast() {
            if (Physics.Raycast(transform.position, -transform.up, out var hit, distance, layer))
                return hit.point + verticalPadding * transform.up;

            return Vector3.zero;
        }
    }
}