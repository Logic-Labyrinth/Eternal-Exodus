using UnityEngine;

public class GameQuit : MonoBehaviour {
    void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}
