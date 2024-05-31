using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntry : MonoBehaviour {
    public ScoreEntry entry;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    public void SetEntry(ScoreEntry entry, int rank) {
        this.entry = entry;

        rankText.text = rank.ToString();
        nameText.text = entry.name;
        scoreText.text = entry.score.ToString();
    }
}