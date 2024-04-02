using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum EnemyType {
    Pawn,
    Bishop,
    Knight,
    Rook
}

public class EnemyAI : MonoBehaviour {
    [SerializeField] private NavMeshAgent agent;
    private GameObject player;
    private HealthSystem playerHealth;
    public EnemyType enemyType = EnemyType.Pawn;

    // Attack parameters
    public float attackRange = 2f; // Radius of the attack sphere
    public int attackDamage = 1; // Amount of damage to deal
    public float attackDuration = 2f; // Duration of the attack in seconds

    private Coroutine attackCoroutine;
    private float clusterRadius = 10f;
    GameObject targetPawnObject;
    private Vector3 bishopTarget;
    public float retreatRange = 20f;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<HealthSystem>();
    }

    void Update() {
        DecisionMaker(enemyType);
    }

    // This is a terrible idea but for the sake of time, fuck it
    void DecisionMaker(EnemyType type) {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (type) {
            case EnemyType.Pawn:
                if (distanceToPlayer < attackRange - 0.5f) {
                    // The player is within a certain distance, trigger the attack
                    if (attackCoroutine != null) {
                        StopCoroutine(attackCoroutine);
                    }
                    attackCoroutine = StartCoroutine(Attack());
                } else {
                    agent.destination = player.transform.position;
                }
                break;
            case EnemyType.Bishop:
                //if player is too close to enemy then run away
                if (distanceToPlayer < retreatRange) {
                    // Calculate the direction away from the player
                    Vector3 directionFromPlayer = (transform.position - player.transform.position).normalized;

                    // Generate a random angle for deviation
                    float angle = Random.Range(-30f, 30f); // Adjust the range as needed

                    // Rotate the direction vector by the random angle
                    Quaternion rotation = Quaternion.Euler(0, angle, 0); // Assuming Y-axis rotation for a typical horizontal plane movement
                    Vector3 randomizedDirection = rotation * directionFromPlayer;

                    // Calculate the retreat target with randomized direction
                    var retreatTarget = player.transform.position + randomizedDirection * retreatRange;
                    // move towards retreat target
                    agent.destination = retreatTarget;
                } else {
                    if (!targetPawnObject) {
                        targetPawnObject = FindClosestClusterOfPawns();
                    }
                    agent.destination = targetPawnObject.transform.position;
                }
                break;
        }
    }

    IEnumerator Attack() {
        float startTime = Time.time;

        while (Time.time - startTime < attackDuration) {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.forward, attackRange);
            foreach (var hitCollider in hitColliders) {
                if (hitCollider.gameObject.GetParent() == player) {
                    playerHealth.TakeDamage(attackDamage, null, hitCollider.transform.position);
                    yield break; // Exit the coroutine early if the player is hit
                }
            }
            yield return null; // Wait for the next frame before continuing the loop
        }
    }

    GameObject FindClosestClusterOfPawns() {
        var pawns = GameObject.FindGameObjectsWithTag("Pawn").ToList();

        // Filter out already selected pawns
        pawns = pawns.Where(pawn => !PawnTargetManager.SelectedPawns.Contains(pawn)).ToList();
        if (pawns.Count == 0) return null; // Early exit if no available pawns

        int largestClusterSize = 0;
        Vector3 clusterCenter = Vector3.zero;
        List<GameObject> largestClusterPawns = new List<GameObject>();

        // Find the largest cluster and its center
        foreach (var pawn in pawns) {
            int clusterSize = 1; // Include the pawn itself
            Vector3 potentialClusterCenter = pawn.transform.position;
            List<GameObject> currentClusterPawns = new List<GameObject> { pawn };

            foreach (var otherPawn in pawns) {
                if (pawn != otherPawn && Vector3.Distance(pawn.transform.position, otherPawn.transform.position) < clusterRadius) {
                    clusterSize++;
                    potentialClusterCenter += otherPawn.transform.position;
                    currentClusterPawns.Add(otherPawn);
                }
            }

            if (clusterSize > largestClusterSize) {
                largestClusterSize = clusterSize;
                clusterCenter = potentialClusterCenter / clusterSize; // Average position of cluster
                largestClusterPawns = new List<GameObject>(currentClusterPawns);
            }
        }

        // Find the pawn closest to the center of the largest cluster
        GameObject targetPawn = null;
        float minDistanceToCenter = float.MaxValue;
        foreach (var pawn in largestClusterPawns) {
            float distanceToCenter = Vector3.Distance(pawn.transform.position, clusterCenter);
            if (distanceToCenter < minDistanceToCenter) {
                minDistanceToCenter = distanceToCenter;
                targetPawn = pawn;
            }
        }

        // Set the target position of the bishop as the position of the closest pawn to the center of the largest cluster
        if (targetPawn != null && PawnTargetManager.TrySelectPawn(targetPawn)) {
            return targetPawn.gameObject;
        }

        return null;
    }

    void OnDrawGizmosSelected() {
        // Draw the attack range when the object is selected in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.forward, attackRange);
    }
}

public static class PawnTargetManager {
    public static List<GameObject> SelectedPawns = new List<GameObject>();

    public static bool TrySelectPawn(GameObject pawn) {
        if (!SelectedPawns.Contains(pawn)) {
            SelectedPawns.Add(pawn);
            return true;
        }
        return false;
    }

    public static void ReleasePawn(GameObject pawn) {
        if (SelectedPawns.Contains(pawn)) {
            SelectedPawns.Remove(pawn);
        }
    }
}