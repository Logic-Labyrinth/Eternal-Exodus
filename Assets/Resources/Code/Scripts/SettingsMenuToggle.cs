using UnityEngine;

public class SettingsMenuToggle : MonoBehaviour {
    SettingsMenuController controller;

    void Awake() {
        controller = FindObjectOfType<SettingsMenuController>(true);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (controller.isOpen) {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                controller.CloseSettings();
            } else {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                controller.OpenSettings();
            }
        }
    }
}