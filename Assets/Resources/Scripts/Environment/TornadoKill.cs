using TEE.Health;
using UnityEngine;

namespace TEE.Environment {
    public class TornadoKill : MonoBehaviour {
        void OnTriggerExit(Collider other) {
            if (other.gameObject.CompareTag("Player")) other.gameObject.GetComponent<PlayerHealthSystem>().Kill();
        }
    }
}