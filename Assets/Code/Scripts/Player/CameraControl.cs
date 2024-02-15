using Sirenix.OdinInspector;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    [BoxGroup("Settings")]
    [MinMaxSlider(-89, 89, true)]
    public Vector2Int cameraPitch = new(-89, 89);
    [BoxGroup("Settings")]
    [SerializeField]
    float sensivity = 3;

    [Space(30)]
    [Title("Extra")]
    [SerializeField]
    Camera cam;
    [SerializeField]
    GameObject settingsUI;

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

            rotX = Mathf.Clamp(rotX, cameraPitch.x, cameraPitch.y);

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
