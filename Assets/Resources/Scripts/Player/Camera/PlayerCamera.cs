using LexUtils.Events;
using UnityEngine;

namespace TEE.Player.Camera {
    public class PlayerCamera : MonoBehaviour {
        [SerializeField] float     sensitivityX = 5f;
        [SerializeField] float     sensitivityY = 5f;
        [SerializeField] Transform orientation;

        float                 rotationX;
        float                 rotationY;
        bool                  disableCameraInput;
        public static Vector2 lookInput = Vector2.zero;

        void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;

            EventForge.Vector2.Get("Input.Player.Look").AddListener(TurnCamera);
        }

        void TurnCamera(Vector2 input) {
            if (disableCameraInput) return;

            float mouseX = input.x;
            float mouseY = input.y;

            rotationY += mouseX * Time.deltaTime * sensitivityX;
            rotationX -= mouseY * Time.deltaTime * sensitivityY;
            rotationX =  Mathf.Clamp(rotationX, -90f, 90f);

            lookInput.x = mouseX;
            lookInput.y = mouseY;

            transform.rotation   = Quaternion.Euler(rotationX, rotationY, 0);
            orientation.rotation = Quaternion.Euler(0,         rotationY, 0);
        }

        public void DisableCameraInput() {
            disableCameraInput = true;
        }

        public void EnableCameraInput() {
            disableCameraInput = false;
        }
    }
}