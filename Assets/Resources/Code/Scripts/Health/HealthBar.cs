using UnityEngine;

public class HealthBar : MonoBehaviour {
    Material material;
    Transform playerTransform;

    private void Awake() {
        playerTransform = Camera.main.transform;
        material = GetComponent<Renderer>().materials[0];
        material.SetFloat("_Progress", 1);
    }

    public void SetProgress(float progress) {
        material.SetFloat("_Progress", progress);
    }

    void LateUpdate() {
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
        transform.Rotate(Vector3.right, -90);
    }
}