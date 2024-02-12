using UnityEngine;

public class CameraControl : MonoBehaviour {
    public float sensivity;
    [Range(-1, -89)]
    public int minX = -60;
    [Range(1, 89)]
    public int maxX = 60;
    public Camera cam;
    public GameObject settingsUI;

    float rotX = 0f;
    float rotY = 0f;
    bool paused = false;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (!paused) {
            rotY += Input.GetAxis("Mouse X") * sensivity;
            rotX += Input.GetAxis("Mouse Y") * sensivity;

            rotX = Mathf.Clamp(rotX, minX, maxX);

            cam.transform.localRotation = Quaternion.Euler(-rotX, 0, 0);
            transform.rotation = Quaternion.Euler(0, rotY, 0);
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            paused = !paused;
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;
            settingsUI.SetActive(paused);
        }
    }
}
