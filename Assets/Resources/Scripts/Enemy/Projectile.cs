using TEE.Health;
using UnityEngine;

namespace TEE.Enemy {
    public class Projectile : MonoBehaviour {
        [SerializeField] float speed        = 20f;
        [SerializeField] float angularSpeed = 1f;
        [SerializeField] int   damage;

        GameObject player;
        LayerMask  groundLayer;
        float      angularSpeedRadians;

        void Awake() {
            player = GameObject.Find("Player");
            if (player == null) Destroy(gameObject);
            groundLayer = LayerMask.NameToLayer("Ground");
        }

        void Start() {
            transform.forward   = Vector3.up;
            angularSpeedRadians = angularSpeed * Mathf.Deg2Rad;
        }

        void FixedUpdate() {
            transform.position += speed * Time.fixedDeltaTime * transform.forward;
            Vector3 direction = Vector3.RotateTowards(transform.forward, player.transform.position - transform.position, angularSpeedRadians, 0f);
            transform.rotation = Quaternion.LookRotation(direction);
        }

        void OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Player")) {
                other.GetComponent<PlayerHealthSystem>().TakeDamage(damage);
            } else if (other.gameObject.layer == groundLayer) {
                Destroy(gameObject);
            }
        }
    }
}