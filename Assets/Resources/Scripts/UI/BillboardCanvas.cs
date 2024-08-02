using UnityEngine;

namespace TEE.UI {
    public class BillboardCanvas : MonoBehaviour {
        Camera mainCamera;

        void Start() {
            mainCamera = Camera.main;
        }

        void Update() {
            gameObject.transform.LookAt(mainCamera.transform);
        }
    }
}