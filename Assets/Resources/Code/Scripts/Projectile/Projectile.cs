using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] float speed = 20f;
    [SerializeField] int damage = 0;
    [SerializeField] float angularSpeed = 1f;

    GameObject player;
    LayerMask groundLayer, playerLayer;
    float angularSpeedRadians;

    void Awake() {
        player = GameObject.Find("Player");
        if (player == null) {
            Debug.LogError("Player not found");
            Destroy(gameObject);
        }

        groundLayer = LayerMask.NameToLayer("Ground");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void Start() {
        // transform.LookAt(player.transform.position);
        transform.forward = Vector3.up;
        angularSpeedRadians = angularSpeed * Mathf.Deg2Rad;
    }

    void FixedUpdate() {
        transform.position += speed * Time.fixedDeltaTime * transform.forward;
        Vector3 direction = Vector3.RotateTowards(transform.forward, player.transform.position - transform.position, angularSpeedRadians, 0f);
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Hit player");
            other.GetComponent<PlayerHealthSystem>().TakeDamage(damage);
        } else if (other.gameObject.layer == groundLayer) {
            Debug.Log("Hit ground");
            Destroy(gameObject);
        }
    }
}
