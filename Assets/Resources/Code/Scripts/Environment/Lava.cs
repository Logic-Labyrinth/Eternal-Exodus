using UnityEngine;

public class Lava : MonoBehaviour {
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponentInParent<HealthSystem>().Kill();
        }
    }
}
