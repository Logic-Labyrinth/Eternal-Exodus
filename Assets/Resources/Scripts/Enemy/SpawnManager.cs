using System;
using System.Collections.Generic;
using System.Linq;
using LexUtils.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TEE.Enemy {
    public class SpawnManager : Singleton<SpawnManager> {
        [SerializeField] GameObject pawnPrefab;
        [SerializeField] GameObject bishopPrefab;
        [SerializeField] GameObject rookPrefab;
        public           int        pawnPoolSize   = 30;
        public           int        bishopPoolSize = 5;
        public           int        rookPoolSize   = 5;

        readonly        Queue<GameObject> pawnQueue   = new();
        readonly        Queue<GameObject> bishopQueue = new();
        readonly        Queue<GameObject> rookQueue   = new();
        public readonly List<GameObject>  PawnList    = new();
        public readonly List<GameObject>  BishopList  = new();
        public readonly List<GameObject>  RookList    = new();

        readonly List<Vector3> spawnPoints = new();

        bool active = true;

        void Start() {
            spawnPoints.AddRange(
                GameObject
                    .FindGameObjectsWithTag("SpawnPoint")
                    .Select(spawnPoint => spawnPoint.transform.position)
            );

            GameObject enemyContainer = new("Enemies") {
                transform = {
                    parent = transform
                }
            };

            // Pawn pool
            for (int i = 0; i < pawnPoolSize; i++) {
                GameObject pawn = Instantiate(pawnPrefab, enemyContainer.transform, true);
                pawnQueue.Enqueue(pawn);
                pawn.SetActive(false);
            }

            // Bishop pool
            for (int i = 0; i < bishopPoolSize; i++) {
                GameObject bishop = Instantiate(bishopPrefab, enemyContainer.transform, true);
                bishopQueue.Enqueue(bishop);
                bishop.SetActive(false);
            }

            // Rook pool
            for (int i = 0; i < rookPoolSize; i++) {
                GameObject rook = Instantiate(rookPrefab, enemyContainer.transform, true);
                rookQueue.Enqueue(rook);
                rook.SetActive(false);
            }
        }

        public void ReloadSpawns() {
            spawnPoints.Clear();
            spawnPoints.AddRange(
                GameObject
                    .FindGameObjectsWithTag("SpawnPoint")
                    .Select(spawnPoint => spawnPoint.transform.position)
            );
        }

        public void SpawnEnemy(EnemyType enemyType, Vector3? spawnPosition = null) {
            // select random spawn point if no spawn position is provided
            if (spawnPosition == null) {
                // if there are no spawn points, break
                if (!spawnPoints.Any()) return;
                spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Count)];
            }

            switch (enemyType) {
                case EnemyType.Pawn:
                    if (!pawnQueue.Any()) return;

                    GameObject pawn = pawnQueue.Dequeue();
                    PawnList.Add(pawn);
                    pawn.transform.position = spawnPosition.Value;
                    pawn.SetActive(true);
                    break;

                case EnemyType.Bishop:
                    if (!bishopQueue.Any()) return;

                    GameObject bishop = bishopQueue.Dequeue();
                    BishopList.Add(bishop);
                    bishop.transform.position = spawnPosition.Value;
                    bishop.SetActive(true);
                    break;

                case EnemyType.Rook:
                    if (!rookQueue.Any()) return;

                    GameObject rook = rookQueue.Dequeue();
                    RookList.Add(rook);
                    rook.transform.position = spawnPosition.Value;
                    rook.SetActive(true);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(enemyType), enemyType, null);
            }
        }

        float timeElapsed;

        void FixedUpdate() {
            if (!active) return;

            timeElapsed += Time.fixedDeltaTime;
            int activePawnCount   = PawnList.Count;
            int activeBishopCount = BishopList.Count;
            int activeRookCount   = RookList.Count;

            switch (timeElapsed) {
                case < 30: {
                    if (activePawnCount   < 10) SpawnEnemy(EnemyType.Pawn);
                    if (activeBishopCount < 1) SpawnEnemy(EnemyType.Bishop);
                    break;
                }
                case >= 30 and <= 60: {
                    if (activePawnCount   < 20) SpawnEnemy(EnemyType.Pawn);
                    if (activeBishopCount < 2) SpawnEnemy(EnemyType.Bishop);
                    if (activeRookCount   < 2) SpawnEnemy(EnemyType.Rook);
                    break;
                }
                case > 60: {
                    if (activePawnCount   < 30) SpawnEnemy(EnemyType.Pawn);
                    if (activeBishopCount < 4) SpawnEnemy(EnemyType.Bishop);
                    if (activeRookCount   < 4) SpawnEnemy(EnemyType.Rook);
                    break;
                }
            }
        }

        public void EnqueueEnemy(GameObject enemy) {
            switch (enemy.tag) {
                case "Pawn":
                    pawnQueue.Enqueue(enemy);
                    PawnList.Remove(enemy);
                    break;

                case "Bishop":
                    bishopQueue.Enqueue(enemy);
                    BishopList.Remove(enemy);
                    break;

                case "Rook":
                    rookQueue.Enqueue(enemy);
                    RookList.Remove(enemy);
                    break;
            }
        }

        public void SetSpawnerActive(bool a) {
            active = a;
        }
    }
}