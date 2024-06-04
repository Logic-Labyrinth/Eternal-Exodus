using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshProUGUI component
using UnityEngine;

// Represents an individual entry in the leaderboard
public class LeaderboardEntry : MonoBehaviour {
    // The score entry associated with this leaderboard entry
    public ScoreEntry entry;
    // The TextMeshProUGUI component to display the rank
    public TextMeshProUGUI rankText;
    // The TextMeshProUGUI component to display the player's name
    public TextMeshProUGUI nameText;
    // The TextMeshProUGUI component to display the player's score
    public TextMeshProUGUI scoreText;

    // Sets the entry's properties to the given score entry and rank
    public void SetEntry(ScoreEntry entry, int rank) {
        this.entry = entry;

        // Set the rank text to the rank as a string
        rankText.text = rank.ToString();
        // Set the name text to the name of the score entry
        nameText.text = entry.name;
        // Set the score text to the score of the score entry as a string
        scoreText.text = entry.score.ToString();
    }
}

// Manages the leaderboard UI
public class LeaderboardList : MonoBehaviour
{
    // The leaderboard entry prefab to instantiate for each leaderboard entry
    [SerializeField] GameObject leaderboardEntryPrefab;
    // The list of score entries to display in the leaderboard
    public List<ScoreEntry> scoreEntries = new List<ScoreEntry>();

    // Called when the script instance is being loaded
    void Awake()
    {
        // Load the leaderboard list
        LoadLeaderboardList();
    }

    // Load the leaderboard list from the leaderboard instance and display it
    void LoadLeaderboardList() {
        // Get the score entries from the leaderboard instance
        scoreEntries = Leaderboard.Instance.GetScore();

        // For each score entry, instantiate a leaderboard entry and set its properties
        for (int i = 0; i < scoreEntries.Count; i++) {
            // Instantiate the leaderboard entry prefab
            GameObject leaderboardEntry = Instantiate(leaderboardEntryPrefab, transform);
            // Set the leaderboard entry's parent to the leaderboard list transform
            leaderboardEntry.transform.parent = transform;
            // Get the leaderboard entry component and set its properties to the score entry and rank
            leaderboardEntry.GetComponent<LeaderboardEntry>().SetEntry(scoreEntries[i], i + 1);
        }
    }

    // Clear the leaderboard list by destroying all its children
    void ClearLeaderboardList() {
        // Destroy all the child objects of the leaderboard list transform
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    // Generate the leaderboard list by clearing it and loading it again
    void GenerateLeaderboardList() {
        // Clear the leaderboard list
        ClearLeaderboardList();
        // Load the leaderboard list
        LoadLeaderboardList();
    }
}

