using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour {
    [SerializeField] TextMeshProUGUI PawnKillCountText;
    [SerializeField] TextMeshProUGUI RookKillCountText;
    [SerializeField] TextMeshProUGUI BishopKillCountText;
    [SerializeField] TextMeshProUGUI SoulCountText;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] GameObject LeaderboardInput;
    [SerializeField] TMP_InputField LeaderboardNameInput;

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
        int totalKills = GameManager.Instance.KillCountPawn + GameManager.Instance.KillCountRook + GameManager.Instance.KillCountBishop;

        PawnKillCountText.text = GameManager.Instance.KillCountPawn.ToString();
        RookKillCountText.text = GameManager.Instance.KillCountRook.ToString();
        BishopKillCountText.text = GameManager.Instance.KillCountBishop.ToString();
        SoulCountText.text = totalKills.ToString();
        ScoreText.text = ((totalKills + Time.timeSinceLevelLoad) * 100).ToString("00");

        if (Leaderboard.Instance.CompareScore(int.Parse(ScoreText.text))) {
            LeaderboardInput.SetActive(true);
        }
    }

    public void SubmitScore() {
        Leaderboard.Instance.AddScore(LeaderboardNameInput.text, int.Parse(ScoreText.text));
        LeaderboardList.GenerateLeaderboardList();
        LeaderboardInput.SetActive(false);
    }
}
