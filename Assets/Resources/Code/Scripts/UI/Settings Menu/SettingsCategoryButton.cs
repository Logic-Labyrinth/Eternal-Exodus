using UnityEngine;

public class SettingsCategoryButton : MonoBehaviour {
    [SerializeField] GameObject menu;

    public void OnClick() {
        SettingsMenuController.Instance.OpenMenu(menu);
    }

    void OnDisable() {
        SettingsMenuController.Instance.CloseMenu(menu);
    }
}
