using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour {
    BoxCollider collider;

    void Start() {
        collider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log(collider.gameObject.name + " " + col.gameObject.transform.name);
    }
}
