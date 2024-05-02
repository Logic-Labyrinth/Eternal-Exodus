using UnityEngine;

public class SettingsMenuToggle : MonoBehaviour {
    SettingsMenuController controller;

    void Awake() {
        controller = FindObjectOfType<SettingsMenuController>(true);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if(controller.isOpen) controller.CloseSettings();
            else controller.OpenSettings();
        }
    }
}
