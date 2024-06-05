using UnityEngine;

public class HammerSmashVFX : MonoBehaviour {
    [SerializeField] float duration = 10;
    [SerializeField] float explosionRadius = 5;

    private void Start() {
        Destroy(gameObject, duration);

        // var collider = GetComponent<SphereCollider>();

        // add explosion at location
        // var rigidbody = gameObject.AddComponent<Rigidbody>();
        // rigidbody.isKinematic = true;
        // rigidbody.AddExplosionForce(20f, transform.position, 10f, 3f, ForceMode.Impulse);
    }
}
