using UnityEngine;

public class SettingsMenuController : MonoBehaviour {
    public static SettingsMenuController Instance { get; private set; }
    GameObject activeMenu;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        DontDestroyOnLoad(this);
    }

    public void OpenMenu(GameObject menu) {
        if (activeMenu) {
            activeMenu.SetActive(false);
        }

        activeMenu = menu;
        activeMenu.SetActive(true);
    }
}
