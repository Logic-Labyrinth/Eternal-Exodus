using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager Instance { get; private set; }

    [SerializeField] GameObject pawnPrefab;
    [SerializeField] GameObject bishopPrefab;
    [SerializeField] GameObject rookPrefab;
    public int PawnPoolSize = 30;
    public int BishopPoolSize = 5;
    public int RookPoolSize = 5;

    readonly Queue<GameObject> pawnQueue = new();
    readonly Queue<GameObject> bishopQueue = new();
    readonly Queue<GameObject> rookQueue = new();
    public readonly List<GameObject> pawnList = new();
    public readonly List<GameObject> bishopList = new();
    public readonly List<GameObject> rookList = new();

    readonly List<Vector3> SpawnPoints = new();

    bool active = true;

    void Awake() {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    void Start() {
        SpawnPoints.AddRange(
            GameObject
                .FindGameObjectsWithTag("SpawnPoint")
                .Select(spawnPoint => spawnPoint.transform.position)
        );

        GameObject enemyContainer = new("Enemies");
        enemyContainer.transform.parent = transform;

        // Pawn pool
        for (int i = 0; i < PawnPoolSize; i++) {
            GameObject pawn = Instantiate(pawnPrefab);
            pawn.transform.parent = enemyContainer.transform;
            pawnQueue.Enqueue(pawn);
            pawn.SetActive(false);
        }

        // Bishop pool
        for (int i = 0; i < BishopPoolSize; i++) {
            GameObject bishop = Instantiate(bishopPrefab);
            bishop.transform.parent = enemyContainer.transform;
            bishopQueue.Enqueue(bishop);
            bishop.SetActive(false);
        }

        // Rook pool
        for (int i = 0; i < RookPoolSize; i++) {
            GameObject rook = Instantiate(rookPrefab);
            rook.transform.parent = enemyContainer.transform;
            rookQueue.Enqueue(rook);
            rook.SetActive(false);
        }
    }

    public void ReloadSpawns() {
        SpawnPoints.Clear();
        SpawnPoints.AddRange(
            GameObject
                .FindGameObjectsWithTag("SpawnPoint")
                .Select(spawnPoint => spawnPoint.transform.position)
        );
    }

    public void SpawnEnemy(EnemyType enemyType, Vector3? spawnPosition = null) {
        // select random spawn point if no spawn position is provided
        if (spawnPosition == null) { // if there are no spawn points, break
            if (!SpawnPoints.Any()) return;
            spawnPosition = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
        }

        switch (enemyType) {
            case EnemyType.Pawn:
                if (!pawnQueue.Any()) return;

                GameObject pawn = pawnQueue.Dequeue();
                pawnList.Add(pawn);
                pawn.transform.position = spawnPosition.Value;
                pawn.SetActive(true);
                break;

            case EnemyType.Bishop:
                if (!bishopQueue.Any()) return;

                GameObject bishop = bishopQueue.Dequeue();
                bishopList.Add(bishop);
                bishop.transform.position = spawnPosition.Value;
                bishop.SetActive(true);
                break;

            case EnemyType.Rook:
                if (!rookQueue.Any()) return;

                GameObject rook = rookQueue.Dequeue();
                rookList.Add(rook);
                rook.transform.position = spawnPosition.Value;
                rook.SetActive(true);
                break;
        }
    }

    float timeElapsed = 0;

    void FixedUpdate() {
        if (!active) return;

        timeElapsed += Time.fixedDeltaTime;
        int activePawnCount = pawnList.Count();
        int activeBishopCount = bishopList.Count();
        int activeRookCount = rookList.Count();

        if (timeElapsed < 30) {
            if (activePawnCount < 10) SpawnEnemy(EnemyType.Pawn);
            if (activeBishopCount < 1) SpawnEnemy(EnemyType.Bishop);

        } else if (timeElapsed >= 30 && timeElapsed <= 60) {
            if (activePawnCount < 20) SpawnEnemy(EnemyType.Pawn);
            if (activeBishopCount < 2) SpawnEnemy(EnemyType.Bishop);
            if (activeRookCount < 2) SpawnEnemy(EnemyType.Rook);

        } else if (timeElapsed > 60) {
            if (activePawnCount < 30) SpawnEnemy(EnemyType.Pawn);
            if (activeBishopCount < 4) SpawnEnemy(EnemyType.Bishop);
            if (activeRookCount < 4) SpawnEnemy(EnemyType.Rook);
        }
    }

    public void EnqueueEnemy(GameObject enemy) {
        switch (enemy.tag) {
            case "Pawn":
                pawnQueue.Enqueue(enemy);
                pawnList.Remove(enemy);
                break;

            case "Bishop":
                bishopQueue.Enqueue(enemy);
                bishopList.Remove(enemy);
                break;

            case "Rook":
                rookQueue.Enqueue(enemy);
                rookList.Remove(enemy);
                break;

            default: // for player
                break;
        }
    }

    public void SetSpawnerActive(bool active) {
        this.active = active;
    }
}
