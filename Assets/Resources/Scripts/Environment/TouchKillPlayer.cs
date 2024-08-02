using TEE.Health;
using UnityEngine;

namespace TEE.Environment {
    public class TouchKillPlayer : MonoBehaviour {
        void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<PlayerHealthSystem>().Kill();
        }
    }
}