using System.Collections.Generic;
using LexUtils.Singleton;
using TEE;
// TextMeshProUGUI component
using UnityEngine;

namespace TEE {
    public class LeaderboardList : PersistentSingleton<LeaderboardList> {
        [SerializeField] GameObject leaderboardEntryPrefab;
        List<ScoreEntry>            scoreEntries = new();

        void Start() {
            LoadLeaderboardList();
        }

        void LoadLeaderboardList() {
            scoreEntries = Leaderboard.Instance.GetScore();

            for (int i = 0; i < scoreEntries.Count; i++) {
                if (i >= 10) break;
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

        public static void GenerateLeaderboardList() {
            Instance.ClearLeaderboardList();
            Instance.LoadLeaderboardList();
        }
    }
}