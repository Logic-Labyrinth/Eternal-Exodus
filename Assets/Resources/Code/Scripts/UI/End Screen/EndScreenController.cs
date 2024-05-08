using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour {
    [SerializeField] int averageTime = 60;
    [SerializeField] TextMeshProUGUI PawnKillCountText;
    [SerializeField] TextMeshProUGUI RookKillCountText;
    [SerializeField] TextMeshProUGUI BishopKillCountText;
    [SerializeField] TextMeshProUGUI SoulCountText;

    public void LoadStartMenu() {
        GameManager.Instance.LoadScene("StartMenu");
    }

    public void ReloadLevel() {
        Time.timeScale = 1f;
        GameManager.Instance.ResetCounter();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() {
        Application.Quit();
    }

    void OnEnable() {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int dif = averageTime - (int)Time.timeSinceLevelLoad;
        if (dif < averageTime / 2) dif = averageTime / 2;
        float percentage = (dif / averageTime / 2) + 1;
        int totalKills = GameManager.Instance.KillCountPawn + GameManager.Instance.KillCountRook + GameManager.Instance.KillCountBishop;
        int score = (int)(totalKills * percentage);

        GameManager.Instance.DisablePlayerInput();

        PawnKillCountText.text = GameManager.Instance.KillCountPawn.ToString();
        RookKillCountText.text = GameManager.Instance.KillCountRook.ToString();
        BishopKillCountText.text = GameManager.Instance.KillCountBishop.ToString();
        SoulCountText.text = totalKills.ToString();
    }
}
