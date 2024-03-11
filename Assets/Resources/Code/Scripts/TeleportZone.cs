using UnityEngine;

public class TeleportZone : MonoBehaviour {
    [SerializeField] GameObject player;
    Vector3 startingPosition;

    private void Start() {
        startingPosition = player.transform.position;
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            player.transform.position = startingPosition;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
