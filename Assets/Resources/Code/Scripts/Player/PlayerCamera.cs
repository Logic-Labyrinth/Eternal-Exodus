using UnityEngine;

public class PlayerCamera : MonoBehaviour {
public static PlayerCamera Instance { get; private set; }
    [SerializeField] float sensitivityX = 5f;
    [SerializeField] float sensitivityY = 5f;
    [SerializeField] Transform orientation;

  
    float rotationX;
    float rotationY;
    bool disableCameraInput = false;
    Vector2 lookInput;
   
// Don't think this is the right way to go about this...
    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

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

        float mouseX = (Input.GetAxisRaw("Mouse X") + Input.GetAxisRaw("Controller X")) * Time.deltaTime * sensitivityX;
        float mouseY = (Input.GetAxisRaw("Mouse Y") + Input.GetAxisRaw("Controller Y")) * Time.deltaTime * sensitivityY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -89f, 89);

        // Replace this if other var ^ can be changed easily
        lookInput.x = mouseX;
        lookInput.y = mouseY;
    }

   


    
    
    public void DisableCameraInput() {
        disableCameraInput = true;
    }

    public void EnableCameraInput() {
        disableCameraInput = false;
    }
}
