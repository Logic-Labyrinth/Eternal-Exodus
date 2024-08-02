using System.Collections.Generic;
using LexUtils.Singleton;
using UnityEngine;
using Newtonsoft.Json;

namespace TEE {
    public class ScoreEntry {
        public readonly string Name;
        public readonly int    Score;

        public ScoreEntry(string name, int score) {
            Name  = name;
            Score = score;
        }
    }

    public class Leaderboard : PersistentSingleton<Leaderboard> {
        List<ScoreEntry> scores   = new();
        public string    fileName = "scores.bin";

        string ScoresPath => Application.persistentDataPath + "/" + fileName;

        protected override void Awake() {
            base.Awake();
            LoadScores();
        }

        public bool CompareScore(int score) {
            GetScore();

            if (scores.Count < 10) return true;

            int lowestScore = scores[9].Score;
            return lowestScore < score;
        }

        public void AddScore(string name, int score) {
            ScoreEntry newScore = new(name, score);
            scores.Add(newScore);
            scores.Sort((x, y) => y.Score.CompareTo(x.Score));

            SerializeScores();
        }

        public List<ScoreEntry> GetScore() {
            LoadScores();
            return scores;
        }

        public void SerializeScores() {
            var json = JsonConvert.SerializeObject(scores);
            Debug.Log(json);
            System.IO.File.WriteAllText(ScoresPath, json);
        }

        public void LoadScores() {
            if (!System.IO.File.Exists(ScoresPath)) return;
            var json = System.IO.File.ReadAllText(ScoresPath);
            scores = JsonConvert.DeserializeObject<List<ScoreEntry>>(json);
        }
    }
}