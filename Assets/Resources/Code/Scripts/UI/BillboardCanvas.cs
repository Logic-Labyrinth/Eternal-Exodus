using UnityEngine;

public class BillboardCanvas : MonoBehaviour {
    Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        gameObject.transform.LookAt(mainCamera.transform);
    }
}
