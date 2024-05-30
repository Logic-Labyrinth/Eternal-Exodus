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

public class LeaderboardList : MonoBehaviour
{
    [SerializeField] GameObject leaderboardEntryPrefab;
    public List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

    // Start is called before the first frame update
    void Awake()
    {
        LoadLeaderboardList();
    }

    void LoadLeaderboardList() {
        scoreEntries = Leaderboard.Instance.GetScore();

        for (int i = 0; i < scoreEntries.Count; i++) {
            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, transform);
            leaderboardEntry.transform.parent = transform;
            leaderboardEntry.GetComponent<LeaderboardEntry>().SetEntry(scoreEntries[i], i + 1);
        }
    }

    void ClearLeaderboardList() {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void GenerateLeaderboardList() {
        ClearLeaderboardList();
        LoadLeaderboardList();
    }
}
