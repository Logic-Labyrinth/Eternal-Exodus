using UnityEngine;

namespace TEE.UI.Controllers {
    public class SettingsMenuToggle : MonoBehaviour {
        SettingsMenuController controller;

        void Awake() {
            controller = FindAnyObjectByType<SettingsMenuController>(FindObjectsInactive.Include);
        }

        void Update() {
            // if (!Input.GetKeyDown(KeyCode.Escape)) return;
            // if (controller.IsOpen) {
            //     Cursor.lockState = CursorLockMode.Locked;
            //     Cursor.visible   = false;
            //     controller.CloseSettings();
            //     return;
            // }
            //
            // Cursor.lockState = CursorLockMode.None;
            // Cursor.visible   = true;
            // controller.OpenSettings();
        }
    }
}