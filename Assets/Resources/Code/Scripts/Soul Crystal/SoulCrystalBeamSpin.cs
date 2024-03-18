using UnityEngine;

public class spinnyboi : MonoBehaviour {
    [SerializeField] float rotationSpeed = 10f;

    void Update() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
