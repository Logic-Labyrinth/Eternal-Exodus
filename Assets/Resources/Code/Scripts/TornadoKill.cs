using UnityEngine;

public class TornadoKill : MonoBehaviour {
    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player")) {
            other.gameObject.GetComponent<PlayerHealthSystem>().Kill();
        }
    }
}
