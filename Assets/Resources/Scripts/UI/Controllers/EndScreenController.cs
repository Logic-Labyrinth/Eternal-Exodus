using LexUtils.Extensions;
using TEE.Enemy;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TEE.UI.Controllers {
    public class EndScreenController : MonoBehaviour {
        [SerializeField] TextMeshProUGUI PawnKillCountText;
        [SerializeField] TextMeshProUGUI RookKillCountText;
        [SerializeField] TextMeshProUGUI BishopKillCountText;
        [SerializeField] TextMeshProUGUI SoulCountText;
        [SerializeField] TextMeshProUGUI ScoreText;
        [SerializeField] GameObject      LeaderboardInput;
        [SerializeField] TMP_InputField  LeaderboardNameInput;

        public void LoadStartMenu() {
            GameManager.Instance.LoadScene("StartMenu");
        }

        public void ReloadLevel() {
            Time.timeScale = 1f;
            GameManager.Instance.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Quit() {
            GameManager.Quit();
        }

        void OnEnable() {
            int totalKills = 0;
            GameManager.KillCounts.ForEach(killCount => totalKills += killCount.Value);

            PawnKillCountText.text   = GameManager.KillCounts[EnemyType.Pawn].ToString();
            BishopKillCountText.text = GameManager.KillCounts[EnemyType.Bishop].ToString();
            RookKillCountText.text   = GameManager.KillCounts[EnemyType.Rook].ToString();
            SoulCountText.text       = totalKills.ToString();
            ScoreText.text           = ((totalKills + Time.timeSinceLevelLoad) * 100).ToString("00");

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
}