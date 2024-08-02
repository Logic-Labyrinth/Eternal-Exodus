using Sirenix.Utilities;
using UnityEngine;

namespace TEE.UI.Controllers {
    public class SettingsMenuController : MonoBehaviour {
        GameObject  activeMenu;
        Transform[] children;
        public bool IsOpen { get; private set; } = false;

        void Awake() {
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
            IsOpen         = true;
            children       = transform.GetChildren(true);
            children.ForEach(x => x.gameObject.SetActive(true));
        }

        public void CloseSettings() {
            Time.timeScale = 1;
            IsOpen         = false;
            children       = transform.GetChildren(true);
            children.ForEach(x => x.gameObject.SetActive(false));
        }

        public void QuitGame() {
            Application.Quit();
        }
    }
}