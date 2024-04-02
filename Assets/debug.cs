using UnityEngine;

public class debug : MonoBehaviour {
    public SettingsMenuController menu;
    bool open = false;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (open) {
                menu.CloseSettings();
                open = false;
            } else {
                menu.OpenSettings();
                open = true;
            }
        }
    }
}
