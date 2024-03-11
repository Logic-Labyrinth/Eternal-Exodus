using UnityEngine;

public class GameQuit : MonoBehaviour {
    void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)) {
            QuitGame();
        }
    }

    public void QuitGame() {
        Application.Quit();
    }
}
