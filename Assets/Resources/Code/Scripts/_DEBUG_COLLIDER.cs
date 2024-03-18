using UnityEngine;

public class _DEBUG_COLLIDER : MonoBehaviour {
    public bool check = false;
    public BoxCollider col;

    private void Awake() {
        col = GetComponent<BoxCollider>();
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("Collision Enter: " + other.gameObject.name);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger Enter: " + other.gameObject.name);
        Debug.Log(other.gameObject.layer);
    }

    private void OnTriggerStay(Collider other) {
        if (check) {
            if (other.gameObject.layer != 3) return;
            Debug.Log("Trigger Stay: " + other.gameObject.name);
            check = false;
        }
    }

    void OnDrawGizmosSelected() {
        // Draw a semitransparent red cube at the transforms position
        if(!col) col = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(col.center + col.transform.position, col.size);
    }
}
