using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ScoreEntry {
    public string name;
    public int score;

    public ScoreEntry(string name, int score) {
        this.name = name;
        this.score = score;
    }
}

public class Leaderboard : MonoBehaviour
{
    List<ScoreEntry> scores = new List<ScoreEntry>();
    public string fileName = "scores.bin";
	string scoresPath { get { return Application.persistentDataPath + "/" + fileName; } }

    public static Leaderboard Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
        DontDestroyOnLoad(gameObject);

        DeserializeScores();
    }

    public void AddScore(string name, int score) {
        scores.Add(new ScoreEntry(name, score));
        scores.Sort((x, y) => y.score.CompareTo(x.score));

        SerializeScores();
    }

    public void SerializeScores() {
        // convert list to json string
        var json = JsonConvert.SerializeObject(scores);
        Debug.Log(json);
        System.IO.File.WriteAllText(scoresPath, json);
    }

    public void DeserializeScores() {
        if (System.IO.File.Exists(scoresPath)) {
            var json = System.IO.File.ReadAllText(scoresPath);
            scores = JsonConvert.DeserializeObject<List<ScoreEntry>>(json);
        }
    }
}
