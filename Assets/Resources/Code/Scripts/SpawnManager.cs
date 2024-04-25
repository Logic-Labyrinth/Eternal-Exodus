using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager spawnManager { get; private set; }

    private List<Vector3> SpawnPoints = new();
    [SerializeField] private GameObject pawnPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject rookPrefab;

    [SerializeField] private int pawnPoolSize = 30;
    [SerializeField] private int bishopPoolSize = 5;
    [SerializeField] private int knightPoolSize = 5;
    [SerializeField] private int rookPoolSize = 5;


    // pool of enemies
    public Queue<GameObject> pawnPool = new Queue<GameObject>();
    public Queue<GameObject> bishopPool = new Queue<GameObject>();
    public Queue<GameObject> knightPool = new Queue<GameObject>();
    public Queue<GameObject> rookPool = new Queue<GameObject>();

    private void Awake() {
        // singleton
        if (spawnManager != null && spawnManager != this) {
            Destroy(this);
        } else {
            spawnManager = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    // Use this function to instantiate enemy pools
    void Start() {
        // Find all spawn points in the scene and store their positions
        SpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint")
            .Select(spawnPoint => spawnPoint.transform.position));

        GameObject enemyContainer = new("Enemies");
        enemyContainer.transform.parent = transform;

        // Instantiate pools for each enemy type
        // (size defined in inspector)
        // and set them to inactive

        // Pawn pool
        for (int i = 0; i < pawnPoolSize; i++) {
            GameObject pawn = Instantiate(pawnPrefab);
            pawn.transform.parent = enemyContainer.transform;
            pawnPool.Enqueue(pawn);
            pawn.SetActive(false);
        }

        // Bishop pool
        for (int i = 0; i < bishopPoolSize; i++) {
            GameObject bishop = Instantiate(bishopPrefab);
            bishop.transform.parent = enemyContainer.transform;
            bishopPool.Enqueue(bishop);
            bishop.SetActive(false);
        }

        // Knight pool
        for (int i = 0; i < knightPoolSize; i++) {
            GameObject knight = Instantiate(knightPrefab);
            knight.transform.parent = enemyContainer.transform;
            knightPool.Enqueue(knight);
            knight.SetActive(false);
        }

        // Rook pool
        for (int i = 0; i < rookPoolSize; i++) {
            GameObject rook = Instantiate(rookPrefab);
            rook.transform.parent = enemyContainer.transform;
            rookPool.Enqueue(rook);
            rook.SetActive(false);
        }
    }

    public void ReloadSpawns() {
        SpawnPoints.Clear();
        SpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint").Select(spawnPoint => spawnPoint.transform.position));
    }

    public void SpawnEnemy(EnemyType enemyType, Vector3? spawnPosition = null) {
        // select random spawn point if no spawn position is provided
        if (spawnPosition == null) {// if there are no spawn points, break
            if (!SpawnPoints.Any()) {
                return;
            }
            spawnPosition = SpawnPoints[Random.Range(0, SpawnPoints.Count)];
        }

        // spawn enemy from correct pool
        switch (enemyType) {
            case EnemyType.Pawn:
                // check if queue is empty
                if (!pawnPool.Any()) {
                    // Debug.Log("No more pawns in the pool");
                    return;
                }
                // get from pool
                GameObject pawn = pawnPool.Dequeue();
                // set position
                pawn.transform.position = spawnPosition.Value;
                // activate enemy
                pawn.SetActive(true);
                break;
            case EnemyType.Bishop:
                if (!bishopPool.Any()) {
                    // Debug.Log("No more bishops in the pool");
                    return;
                }
                GameObject bishop = bishopPool.Dequeue();
                bishop.transform.position = spawnPosition.Value;
                bishop.SetActive(true);
                break;
            case EnemyType.Knight:
                if (!knightPool.Any()) {
                    // Debug.Log("No more knights in the pool");
                    return;
                }
                GameObject knight = knightPool.Dequeue();
                knight.transform.position = spawnPosition.Value;
                knight.SetActive(true);
                break;
            case EnemyType.Rook:
                if (!rookPool.Any()) {
                    // Debug.Log("No more rooks in the pool");
                    return;
                }
                GameObject rook = rookPool.Dequeue();
                rook.transform.position = spawnPosition.Value;
                rook.SetActive(true);
                break;
        }
    }

    private void FixedUpdate() {
        // SpawnEnemy(EnemyType.Pawn);
        if (pawnPool.Count > 10) {
            SpawnEnemy(EnemyType.Pawn);
        }
        if (bishopPool.Count > 0) {
            SpawnEnemy(EnemyType.Bishop);
        }
        if (rookPool.Count > 0) {
            SpawnEnemy(EnemyType.Rook);
        }
    }

    public void EnqueueEnemy(GameObject enemy) {
        // find enemy type and enqueue it
        switch (enemy.tag) {
            case "Pawn":
                PawnTargetManager.ReleasePawn(enemy);
                pawnPool.Enqueue(enemy);
                break;
            case "Bishop":
                bishopPool.Enqueue(enemy);
                break;
            case "Knight":
                knightPool.Enqueue(enemy);
                break;
            case "Rook":
                rookPool.Enqueue(enemy);
                break;
            default: // for player
                break;
        }
    }
}
