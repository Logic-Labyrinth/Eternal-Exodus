using TMPro;
using UnityEngine;

public class EndScreenController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI PawnKillCountText;
    [SerializeField] TextMeshProUGUI KnightKillCountText;
    [SerializeField] TextMeshProUGUI RookKillCountText;
    [SerializeField] TextMeshProUGUI BishopKillCountText;

    public void LoadStartMenu() {
        GameManager.Instance.LoadScene("StartMenu");
    }

    void OnEnable() {
        // Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.DisablePlayerInput();

        PawnKillCountText.text = (GameManager.Instance.KillCountPawn - 1).ToString();
        KnightKillCountText.text = (GameManager.Instance.KillCountKnight - 1).ToString();
        RookKillCountText.text = (GameManager.Instance.KillCountRook - 1).ToString();
        BishopKillCountText.text = (GameManager.Instance.KillCountBishop - 1).ToString();
    }
}
