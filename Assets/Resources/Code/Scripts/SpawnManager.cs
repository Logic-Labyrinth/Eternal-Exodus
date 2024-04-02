using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    public static SpawnManager spawnManager { get; private set; }

    private List<Vector3> SpawnPoints = new List<Vector3>();
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

        // Instantiate pools for each enemy type
        // (size defined in inspector)
        // and set them to inactive

        // Pawn pool
        for (int i = 0; i < pawnPoolSize; i++) {
            GameObject pawn = Instantiate(pawnPrefab);
            pawnPool.Enqueue(pawn);
            pawn.SetActive(false);
        }

        // Bishop pool
        for (int i = 0; i < bishopPoolSize; i++) {
            GameObject bishop = Instantiate(bishopPrefab);
            bishopPool.Enqueue(bishop);
            bishop.SetActive(false);
        }

        // Knight pool
        for (int i = 0; i < knightPoolSize; i++) {
            GameObject knight = Instantiate(knightPrefab);
            knightPool.Enqueue(knight);
            knight.SetActive(false);
        }

        // Rook pool
        for (int i = 0; i < rookPoolSize; i++) {
            GameObject rook = Instantiate(rookPrefab);
            rookPool.Enqueue(rook);
            rook.SetActive(false);
        }
    }

    public void ReloadSpawns() {
        SpawnPoints.Clear();
        SpawnPoints.AddRange(GameObject.FindGameObjectsWithTag("SpawnPoint").Select(spawnPoint => spawnPoint.transform.position));
    }

    public IEnumerator SpawnEnemy(EnemyType enemyType, float waitTime) {

        // if there are no spawn points, return
        if (!SpawnPoints.Any()) {
            yield return null;
        }

        // select random spawn point
        Vector3 spawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Count)];

        // spawn enemy from correct pool
        switch (enemyType) {
            case EnemyType.Pawn:
                // check if queue is empty
                if (!pawnPool.Any()) {
                    yield break;
                }
                // get from pool
                GameObject pawn = pawnPool.Dequeue();
                // set position
                pawn.transform.position = spawnPoint;
                // activate enemy
                pawn.SetActive(true);
                break;
            case EnemyType.Bishop:
                if (!bishopPool.Any()) {
                    yield break;
                }
                GameObject bishop = bishopPool.Dequeue();
                bishop.transform.position = spawnPoint;
                bishop.SetActive(true);
                break;
            case EnemyType.Knight:
                if (!knightPool.Any()) {
                    yield break;
                }
                GameObject knight = knightPool.Dequeue();
                knight.transform.position = spawnPoint;
                knight.SetActive(true);
                break;
            case EnemyType.Rook:
                if (!rookPool.Any()) {
                    yield break;
                }
                GameObject rook = rookPool.Dequeue();
                rook.transform.position = spawnPoint;
                rook.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(waitTime);
    }

    private void FixedUpdate() {
        var pawnSpawner = StartCoroutine(SpawnEnemy(EnemyType.Pawn, 5f));
        var bishopSpawner = StartCoroutine(SpawnEnemy(EnemyType.Bishop, 10f));
    }

    public void EnqueueEnemy(GameObject enemy) {
        // find enemy type and enqueue it
        switch (enemy.gameObject.tag) {
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
