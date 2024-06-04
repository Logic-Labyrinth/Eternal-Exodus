using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField] float sensitivityX = 5f;
    [SerializeField] float sensitivityY = 5f;
    [SerializeField] Transform orientation;

  
    float rotationX;
    float rotationY;
    bool disableCameraInput = false;
    public static Vector2 lookInput;
   

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        if (disableCameraInput) return;

        GetInput();

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    void GetInput() {

        float mouseX = Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X");
        float mouseY = Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y");

        rotationY += mouseX * Time.deltaTime * sensitivityX;
        rotationX -= mouseY * Time.deltaTime * sensitivityY;
        rotationX = Mathf.Clamp(rotationX, -89f, 89);

        // Replace this if other var ^ can be changed easily
        lookInput.x = mouseX;
        lookInput.y = mouseY;

        Debug.Log(lookInput);
    }

   
    public void DisableCameraInput() {
        disableCameraInput = true;
    }

    public void EnableCameraInput() {
        disableCameraInput = false;
    }
}
