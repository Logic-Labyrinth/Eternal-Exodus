using TEE.Input;
using UnityEngine;

namespace TEE.Player.Camera {
    public class PlayerCamera : MonoBehaviour {
        [SerializeField] float   sensitivityX  = 5f;
        [SerializeField] float   sensitivityY  = 5f;
        [SerializeField] Vector2 verticalClamp = new(-90, 90);

        float rotationX;
        float rotationY;

        void Update() {
            TurnCamera();
        }

        void TurnCamera() {
            Vector2 input  = InputManager.GetLookInput();
            float   mouseX = input.x;
            float   mouseY = input.y;

            rotationY += mouseX * sensitivityX * Time.deltaTime;
            rotationX -= mouseY * sensitivityY * Time.deltaTime;
            rotationX =  Mathf.Clamp(rotationX, verticalClamp.x, verticalClamp.y);

            transform.localRotation            = Quaternion.Euler(rotationX, 0,         0);
            Movement.Player.Transform.rotation = Quaternion.Euler(0,         rotationY, 0);
        }
    }
}