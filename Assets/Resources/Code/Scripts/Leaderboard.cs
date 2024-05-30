using System; // Import the System namespace, which contains basic types and classes for system functionality.
using System.Collections; // Import the Collections namespace, which contains classes for generic collections.
using System.Collections.Generic; // Import the Generic Collections namespace, which contains classes for generic collections.
using UnityEngine; // Import the UnityEngine namespace, which contains classes for working with Unity game engine.
using Newtonsoft.Json; // Import the Newtonsoft.Json namespace, which contains classes for working with JSON data.

// Define a class called ScoreEntry. This class represents an entry in the leaderboard.
public class ScoreEntry {
    public string name; // The name of the player.
    public int score; // The score of the player.

    // Constructor for the ScoreEntry class. Initializes the name and score fields.
    public ScoreEntry(string name, int score) {
        this.name = name;
        this.score = score;
    }
}

// Define a class called Leaderboard. This class represents the leaderboard.
public class Leaderboard : MonoBehaviour {
    List<ScoreEntry> scores = new List<ScoreEntry>(); // The list of scores.
    public string fileName = "scores.bin"; // The name of the file where the scores are stored.
    string scoresPath { get { return Application.persistentDataPath + "/" + fileName; } } // The path to the scores file.

    public static Leaderboard Instance { get; private set; } // The singleton instance of the Leaderboard class.

    // The Awake() method is called when the script instance is being loaded.
    private void Awake() {
        // Check if there is already an instance of the Leaderboard class.
        if (Instance != null && Instance != this) {
            // If there is an instance, destroy this instance.
            Destroy(this);
        } else {
            // If this is the first instance, set it as the singleton instance.
            Instance = this;
        }
        // Make the Leaderboard class persist across scene loads.
        DontDestroyOnLoad(gameObject);

        // Load the scores from the file.
        LoadScores();
    }

    // The AddScore() method adds a new score to the leaderboard.
    public void AddScore(string name, int score) {
        // Create a new ScoreEntry object with the given name and score.
        ScoreEntry newScore = new ScoreEntry(name, score);
        // Add the new score to the list of scores.
        scores.Add(newScore);
        // Sort the list of scores in descending order based on the score.
        scores.Sort((x, y) => y.score.CompareTo(x.score));

        // Serialize the scores to the file.
        SerializeScores();
    }

    // The GetScore() method returns the list of scores.
    public List<ScoreEntry> GetScore() {
        // Load the scores from the file.
        LoadScores();
        // Return the list of scores.
        return scores;
    }

    // The SerializeScores() method serializes the scores to the file.
    public void SerializeScores() {
        // Convert the list of scores to a JSON string.
        var json = JsonConvert.SerializeObject(scores);
        // Print the JSON string to the debug log.
        Debug.Log(json);
        // Write the JSON string to the file.
        System.IO.File.WriteAllText(scoresPath, json);
    }

    // The LoadScores() method loads the scores from the file.
    public void LoadScores() {
        // Check if the file exists.
        if (System.IO.File.Exists(scoresPath)) {
            // Read the contents of the file.
            var json = System.IO.File.ReadAllText(scoresPath);
            // Deserialize the JSON string to a List of ScoreEntry objects.
            scores = JsonConvert.DeserializeObject<List<ScoreEntry>>(json);
        }
    }
}

