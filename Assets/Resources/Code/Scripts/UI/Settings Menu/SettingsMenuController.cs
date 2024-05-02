using Sirenix.Utilities;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour {
    // public static SettingsMenuController Instance { get; private set; }
    GameObject activeMenu;
    Transform[] children;
    public bool isOpen { get; private set; } = false;

    void Awake() {
        // if (Instance != null && Instance != this) Destroy(this);
        // else Instance = this;

        // DontDestroyOnLoad(this);

        children = transform.GetChildren(true);
        children.ForEach(x => x.gameObject.SetActive(false));
    }

    public void OpenMenu(GameObject menu) {
        if (activeMenu) activeMenu.SetActive(false);
        activeMenu = menu;
        activeMenu.SetActive(true);
    }

    public void CloseMenu(GameObject menu) {
        menu.SetActive(false);
    }

    public void OpenSettings() {
        Time.timeScale = 0;
        isOpen = true;
        children.ForEach(x => x.gameObject.SetActive(true));
    }

    public void CloseSettings() {
        Time.timeScale = 1;
        isOpen = false;
        children.ForEach(x => x.gameObject.SetActive(false));
    }

    public void QuitGame() {
        Application.Quit();
    }
}
