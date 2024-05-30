using UnityEngine;

public class SettingsMenuToggle : MonoBehaviour {
    SettingsMenuController controller;

    void Awake() {
        controller = FindObjectOfType<SettingsMenuController>(true);
    }

    void Update() {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (controller.isOpen) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controller.CloseSettings();
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        controller.OpenSettings();
    }
}
