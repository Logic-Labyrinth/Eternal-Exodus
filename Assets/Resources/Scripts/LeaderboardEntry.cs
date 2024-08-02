using TEE;
using TMPro;
using UnityEngine;

namespace TEE {
    public class LeaderboardEntry : MonoBehaviour {
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI scoreText;

        public void SetEntry(ScoreEntry entry, int rank) {
            rankText.text  = rank.ToString();
            nameText.text  = entry.Name;
            scoreText.text = entry.Score.ToString();
        }
    }
}