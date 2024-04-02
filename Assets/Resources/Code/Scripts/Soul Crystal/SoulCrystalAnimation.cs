using UnityEngine;

public class SoulCrystalAnimation : MonoBehaviour {
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float bobSpeed = 0.5f;
    [SerializeField] float bobHeight = 0.5f;

    void Update() {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight * Time.deltaTime, transform.position.z);
    }
}
