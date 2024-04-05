using UnityEngine;

public class debug : MonoBehaviour {
    CapsuleCollider col;

    private void Start() {
        col = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision other) {
        Debug.LogError("Enter: " + other.gameObject.name);
    }

    private void OnCollisionStay(Collision other) {
        Debug.LogError("Stay: " + other.gameObject.name);
    }

    private void OnCollisionExit(Collision other) {
        Debug.LogError("Exit: " + other.gameObject.name);
    }
}
