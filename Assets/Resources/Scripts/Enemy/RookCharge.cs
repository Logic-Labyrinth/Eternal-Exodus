using TEE.Health;
using UnityEngine;

namespace TEE.Enemy {
    public class RookCharge : MonoBehaviour {
        public           bool  isCharging;
        [SerializeField] float verticalForce   = 40;
        [SerializeField] float horizontalForce = 100;

        void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player") || !isCharging) return;

            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * horizontalForce + Vector3.up * verticalForce, ForceMode.Impulse);
            other.gameObject.GetComponent<PlayerHealthSystem>().TakeDamage(30);
            isCharging = false;
        }
    }
}