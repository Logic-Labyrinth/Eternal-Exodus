using UnityEngine;

namespace TEE.Environment {
    public class TeleportZone : MonoBehaviour {
        [SerializeField] GameObject player;
        Vector3                     startingPosition;

        void Start() {
            startingPosition = player.transform.position;
        }

        void OnTriggerExit(Collider other) {
            if (!other.gameObject.CompareTag("Player")) return;
            player.transform.position                 = startingPosition;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}