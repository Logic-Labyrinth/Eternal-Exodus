using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI PawnKillCountText;
    [SerializeField] TextMeshProUGUI RookKillCountText;
    [SerializeField] TextMeshProUGUI BishopKillCountText;

    public void LoadStartMenu() {
        GameManager.Instance.LoadScene("StartMenu");
    }

    public void ReloadLevel() {
        Time.timeScale = 1f;
        GameManager.Instance.ResetCounter();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable() {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.DisablePlayerInput();

        PawnKillCountText.text = (GameManager.Instance.KillCountPawn - 1).ToString();
        RookKillCountText.text = (GameManager.Instance.KillCountRook - 1).ToString();
        BishopKillCountText.text = (GameManager.Instance.KillCountBishop - 1).ToString();
    }
}
