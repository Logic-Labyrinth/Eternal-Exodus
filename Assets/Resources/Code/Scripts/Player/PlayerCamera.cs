using UnityEngine;

public class PlayerCamera : MonoBehaviour {
    [SerializeField] float sensitivityX = 5f;
    [SerializeField] float sensitivityY = 5f;
    [SerializeField] Transform orientation;

    float rotationX;
    float rotationY;
    bool disableCameraInput = false;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (disableCameraInput) return;

        float mouseX = (Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X")) * Time.deltaTime * sensitivityX;
        float mouseY = (Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y")) * Time.deltaTime * sensitivityY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -89f, 89);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    public void DisableCameraInput() {
        disableCameraInput = true;
        Debug.Log("COCK");
    }

    public void EnableCameraInput() {
        disableCameraInput = false;
    }
}
