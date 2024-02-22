using UnityEngine;

public class PlayerCamera : MonoBehaviour {
  [SerializeField]
  float sensitivityX = 5f;
  [SerializeField]
  float sensitivityY = 5f;
  [SerializeField]
  Transform orientation;

  float rotationX;
  float rotationY;

  private void Start() {
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
  }

  private void Update() {
    float mouseX = (Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X")) * Time.deltaTime * sensitivityX;
    float mouseY = (Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y")) * Time.deltaTime * sensitivityY;

    rotationY += mouseX;
    rotationX -= mouseY;
    rotationX = Mathf.Clamp(rotationX, -90f, 90f);

    transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    orientation.rotation = Quaternion.Euler(0, rotationY, 0);
  }
}