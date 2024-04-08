using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public enum EnemyType
{
    Pawn,
    Bishop,
    Knight,
    Rook
}

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent agent;
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

    [ShowIf("enemyType", EnemyType.Bishop)]
    public float retreatRange = 20f;
    float lastSummonTime;
    float summonCooldownTime = 20f;

    [SerializeField]
    LayerMask groundLayer;

    [ShowIf("enemyType", EnemyType.Rook)]
    public float chargeSpeed = 20f;

    private SpawnManager spawnManager;
    private float checkInterval;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<HealthSystem>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        checkInterval = Random.Range(0f, 0.2f);
    }

    void OnEnable()
    {
        lastSummonTime = Time.time;
        checkInterval = Random.Range(0f, 0.2f);
    }

    // Time between checks in seconds
    private float lastCheckTime = 0f;

    void Update()
    {
        // Reduce frequency of checks
        if (Time.time >= lastCheckTime + checkInterval)
        {
            DecisionMaker(enemyType);
            lastCheckTime = Time.time;
            checkInterval = Random.Range(0f, 0.2f);
        }
    }

    void DecisionMaker(EnemyType type)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (type)
        {
            case EnemyType.Pawn:
                HandlePawnBehavior(distanceToPlayer);
                break;
            case EnemyType.Bishop:
                HandleBishopBehavior(distanceToPlayer);
                break;
            case EnemyType.Rook:
                HandleRookBehavior(distanceToPlayer);
                break;
        }
    }

    void HandlePawnBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer < attackRange - 0.5f)
        {
            TriggerAttack();
        }
        else
        {
            agent.destination = player.transform.position;
        }
    }

    void TriggerAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(Attack());
    }

    void HandleBishopBehavior(float distanceToPlayer)
    {
        if (distanceToPlayer < retreatRange)
        {
            RetreatFromPlayer();
        }
        else if (Time.time >= lastSummonTime + summonCooldownTime)
        {
            lastSummonTime = Time.time;
            StartCoroutine(SummonPawns(spawnManager));
        }
        else
        {
            MoveToTargetPawn();
        }
    }

    void RetreatFromPlayer()
    {
        // Calculate the direction away from the player
        Vector3 directionFromPlayer = (transform.position - player.transform.position).normalized;

        // Generate a random angle for deviation
        float angle = Random.Range(-90f, 90f); // Adjust the range as needed

        // Rotate the direction vector by the random angle
        Quaternion rotation = Quaternion.Euler(0, angle, 0); // Assuming Y-axis rotation for a typical horizontal plane movement
        Vector3 randomizedDirection = rotation * directionFromPlayer;

        // Calculate the retreat target with randomized direction
        var retreatTarget = player.transform.position + randomizedDirection * retreatRange;
        // move towards retreat target
        agent.destination = retreatTarget;
    }

    void MoveToTargetPawn()
    {
        // Logic to move towards the closest cluster of pawns, optimizing by only recalculating when necessary
        if (targetPawnObject == null || !PawnTargetManager.SelectedPawns.Contains(targetPawnObject))
        {
            targetPawnObject = FindClosestPawnCluster();
        }

        if (targetPawnObject == null)
            return;

        agent.destination = targetPawnObject.transform.position;
    }

    IEnumerator SummonPawns(SpawnManager spawnManager)
    {
        if (spawnManager == null)
        {
            Debug.LogError($"{nameof(EnemyAI)} tried to summon pawns but spawnManager is null");
            yield break;
        }

        lastSummonTime = Time.time;

        // Spawn 5 pawns, one at a time, with a slight delay between each spawn
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomPoint;
            try
            {
                randomPoint = transform.position + Random.insideUnitSphere * 10f;
            }
            catch (System.Exception e)
            {
                Debug.LogError(
                    $"{nameof(EnemyAI)} failed to generate a random point for summoning pawns: {e}"
                );
                yield break;
            }

            randomPoint.y = transform.position.y;

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                Debug.LogError(
                    $"{nameof(EnemyAI)} could not find a valid position to summon pawns"
                );
                yield break;
            }

            spawnManager.SpawnEnemy(EnemyType.Pawn, hit.position);

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
    }

    IEnumerator Attack()
    {
        float startTime = Time.time;

        while (Time.time - startTime < attackDuration)
        {
            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position + Vector3.forward,
                attackRange
            );
            if (hitColliders == null)
            {
                yield break; // Exit the coroutine early if hitColliders is null
            }

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider == null)
                {
                    continue; // Ignore null entries in hitColliders
                }

                GameObject parentGameObject = hitCollider.gameObject.GetParent();
                if (parentGameObject == null)
                {
                    continue; // Ignore null entries in hitColliders
                }

                if (parentGameObject == player)
                {
                    // playerHealth could be null, so we need to handle that case
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(attackDamage, null, hitCollider.transform.position);
                    }
                    yield break; // Exit the coroutine early if the player is hit
                }
            }
            yield return null; // Wait for the next frame before continuing the loop
        }
    }

    GameObject FindClosestPawnCluster()
    {
        var pawns = GameObject.FindGameObjectsWithTag("Pawn").ToList();

        // Filter out already selected pawns
        pawns = pawns.Where(pawn => !PawnTargetManager.SelectedPawns.Contains(pawn)).ToList();
        if (pawns.Count == 0)
            return null; // Early exit if no available pawns

        int largestClusterSize = 0;
        Vector3 clusterCenter = Vector3.zero;
        List<GameObject> largestClusterPawns = new List<GameObject>();

        // Find the largest cluster and its center
        foreach (var pawn in pawns)
        {
            int clusterSize = 1; // Include the pawn itself
            Vector3 potentialClusterCenter = pawn.transform.position;
            List<GameObject> currentClusterPawns = new List<GameObject> { pawn };

            foreach (var otherPawn in pawns)
            {
                if (
                    pawn != otherPawn
                    && Vector3.Distance(pawn.transform.position, otherPawn.transform.position)
                        < clusterRadius
                )
                {
                    clusterSize++;
                    potentialClusterCenter += otherPawn.transform.position;
                    currentClusterPawns.Add(otherPawn);
                }
            }

            if (clusterSize > largestClusterSize)
            {
                largestClusterSize = clusterSize;
                clusterCenter = potentialClusterCenter / clusterSize; // Average position of cluster
                largestClusterPawns = new List<GameObject>(currentClusterPawns);
            }
        }

        // Find the pawn closest to the center of the largest cluster
        GameObject targetPawn = null;
        float minDistanceToCenter = float.MaxValue;
        foreach (var pawn in largestClusterPawns)
        {
            float distanceToCenter = Vector3.Distance(pawn.transform.position, clusterCenter);
            if (distanceToCenter < minDistanceToCenter)
            {
                minDistanceToCenter = distanceToCenter;
                targetPawn = pawn;
            }
        }

        // Set the target position of the bishop as the position of the closest pawn to the center of the largest cluster
        if (targetPawn != null && PawnTargetManager.TrySelectPawn(targetPawn))
        {
            return targetPawn.gameObject;
        }

        return null;
    }

    bool isCharging = false;

    private void HandleRookBehavior(float distanceToPlayer)
    {
        RaycastHit hit;
        if (distanceToPlayer < attackRange - 0.5f)
        {
            TriggerAttack();
        }
        else if (
            !Physics.Raycast(
                transform.position,
                player.transform.position - transform.position,
                out hit,
                distanceToPlayer,
                groundLayer
            ) && !isCharging
        )
        {
            // Charge at player
            StartCoroutine(ChargeTowardsPlayer());
        }
        else
        {
            agent.destination = player.transform.position;
        }
    }

    private IEnumerator ChargeTowardsPlayer()
    {
        isCharging = true;
        Vector3 initialPosition = transform.position; // Record the starting position
        Vector3 targetPosition = player.transform.position; // Initial target position
        float totalChargeTime = Vector3.Distance(initialPosition, targetPosition) / chargeSpeed; // Total time to complete the charge
        float startTime = Time.time;
        float minimumDistanceToPlayer = 1f; // Threshold distance to stop the charge if close enough to the player

        while (isCharging)
        {
            float elapsedTime = Time.time - startTime;
            float fracJourney = elapsedTime / totalChargeTime;

            // Dynamically update target position for the first 25% of the charge
            if (fracJourney <= 0.25f)
            {
                targetPosition = player.transform.position;
                totalChargeTime = Vector3.Distance(initialPosition, targetPosition) / chargeSpeed; // Recalculate total charge time based on new target
            }

            // Calculate the new position using Lerp and update the enemy's position
            Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, fracJourney);
            transform.position = newPosition;

            // Aim the gameObject towards the target continuously during the first part of the charge
            if (fracJourney <= 0.25f)
            {
                transform.LookAt(targetPosition);
            }

            // Check if the enemy is close enough to the player to stop the charge
            if (
                Vector3.Distance(transform.position, player.transform.position)
                <= minimumDistanceToPlayer
            )
            {
                Debug.Log("Player hit by charge");
                isCharging = false;
            }

            // Check for collision with groundLayer to potentially stop the charge
            if (
                Physics.CheckCapsule(
                    transform.position,
                    transform.position + Vector3.up * 0.5f,
                    0.2f,
                    groundLayer
                )
            )
            {
                isCharging = false; // Exit the loop if collision detected
            }

            // Condition to end the charge if the duration has been reached
            if (fracJourney >= 1f)
            {
                isCharging = false;
            }

            yield return null;
        }

        // Set rook to nearest point on navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
    }
}

public static class PawnTargetManager
{
    public static List<GameObject> SelectedPawns = new List<GameObject>();

    public static bool TrySelectPawn(GameObject pawn)
    {
        if (!SelectedPawns.Contains(pawn))
        {
            SelectedPawns.Add(pawn);
            return true;
        }
        return false;
    }

    public static void ReleasePawn(GameObject pawn)
    {
        if (SelectedPawns.Contains(pawn))
        {
            SelectedPawns.Remove(pawn);
        }
    }
}
