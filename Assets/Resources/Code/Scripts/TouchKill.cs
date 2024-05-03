using UnityEngine;

public class TouchKill : MonoBehaviour {
    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerHealthSystem>().Kill();
        }
    }
}
