using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyType {
    Pawn,
    Bishop,
    Knight,
    Rook
}

public class EnemyAI : MonoBehaviour {
    [SerializeField]
    NavMeshAgent agent;
    GameObject player;
    PlayerHealthSystem playerHealth;
    public EnemyType enemyType = EnemyType.Pawn;

    // Attack parameters
    public float attackRange = 2f; // Radius of the attack sphere
    public int attackDamage = 1; // Amount of damage to deal
    public float attackDuration = 2f; // Duration of the attack in seconds

    Coroutine attackCoroutine;
    float clusterRadius = 10f;
    GameObject targetPawnObject;

    [ShowIf("enemyType", EnemyType.Bishop)]
    public float retreatRange = 20f;
    public float projectileCooldown = 1f;
    [SerializeField] GameObject projectilePrefab;
    float lastSummonTime;
    float summonCooldownTime = 20f;
    bool canAttack = true;

    [SerializeField] LayerMask groundLayer;

    [ShowIf("enemyType", EnemyType.Rook)]
    public float chargeSpeed = 20f;
    bool chargingCooldown = false;
    public float chargingCooldownTime = 10f;

    SpawnManager spawnManager;
    float checkInterval;

    public Animator animator;
    public Sound soundEffect;

    new Rigidbody rigidbody;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        playerHealth = player.GetComponent<PlayerHealthSystem>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        checkInterval = Random.Range(0f, 0.1f);

        // InvokeRepeating(nameof(GroundCheck), 0f, 1f);
        rigidbody = GetComponent<Rigidbody>();
    }

    void OnEnable() {
        lastSummonTime = Time.time;
        checkInterval = Random.Range(0f, 0.1f);
    }

    // Time between checks in seconds
    float lastCheckTime = 0f;

    void FixedUpdate() {
        // Reduce frequency of checks
        if (Time.time >= lastCheckTime + checkInterval) {
            DecisionMaker(enemyType);
            lastCheckTime = Time.time;
            checkInterval = Random.Range(0f, 0.2f);
        }

        // if (enemyType == EnemyType.Pawn) {
        //     animator.SetFloat("Speed", agent.velocity.magnitude);
        // }
        GroundCheck();
    }

    void DecisionMaker(EnemyType type) {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (type) {
            // case EnemyType.Pawn:
            //     HandlePawnBehavior(distanceToPlayer);
            //     break;
            case EnemyType.Bishop:
                HandleBishopBehavior(distanceToPlayer);
                break;
            case EnemyType.Rook:
                HandleRookBehavior(distanceToPlayer);
                break;
        }
    }

    // void HandlePawnBehavior(float distanceToPlayer) {
    //     if (distanceToPlayer < attackRange - 0.5f) {
    //         TriggerAttack();
    //     } else {
    //         agent.destination = player.transform.position;
    //     }
    // }

    void TriggerAttack() {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(Attack());
    }

    void HandleBishopBehavior(float distanceToPlayer) {
        if (distanceToPlayer < retreatRange) {
            // RetreatFromPlayer();
        } else if (Time.time >= lastSummonTime + summonCooldownTime) {
            lastSummonTime = Time.time;
            StartCoroutine(SummonPawns(spawnManager));
        } else {
            MoveToTargetPawn();
            BishopAttackPlayer();
        }
    }

    void BishopAttackPlayer() {
        if (!canAttack) return;
        canAttack = false;
        Instantiate(projectilePrefab, transform.position + Vector3.up * 3.5f, Quaternion.identity);
        StartCoroutine(ResetProjectileCooldown());
    }

    IEnumerator ResetProjectileCooldown() {
        yield return new WaitForSeconds(projectileCooldown);
        canAttack = true;
    }

    // void RetreatFromPlayer() {
    //     // Calculate the direction away from the player
    //     Vector3 directionFromPlayer = (transform.position - player.transform.position).normalized;

    //     // Generate a random angle for deviation
    //     float angle = Random.Range(-90f, 90f); // Adjust the range as needed

    //     // Rotate the direction vector by the random angle
    //     Quaternion rotation = Quaternion.Euler(0, angle, 0); // Assuming Y-axis rotation for a typical horizontal plane movement
    //     Vector3 randomizedDirection = rotation * directionFromPlayer;

    //     // Calculate the retreat target with randomized direction
    //     var retreatTarget = player.transform.position + randomizedDirection * retreatRange;
    //     // move towards retreat target
    //     agent.destination = retreatTarget;
    // }

    void MoveToTargetPawn() {
        // Logic to move towards the closest cluster of pawns, optimizing by only recalculating when necessary
        if (targetPawnObject == null || !PawnTargetManager.SelectedPawns.Contains(targetPawnObject)) {
            targetPawnObject = FindClosestPawnCluster();
        }

        if (targetPawnObject == null)
            return;

        agent.destination = targetPawnObject.transform.position;
    }

    IEnumerator SummonPawns(SpawnManager spawnManager) {
        if (spawnManager == null) {
            Debug.LogError($"{nameof(EnemyAI)} tried to summon pawns but spawnManager is null");
            yield break;
        }

        // disable movement
        agent.isStopped = true;

        // look at player
        transform.LookAt(player.transform);

        // Play summon animation
        animator.SetTrigger("SummonPawns");

        yield return new WaitForSeconds(1f);

        lastSummonTime = Time.time;

        // Spawn 5 pawns, one at a time, with a slight delay between each spawn
        for (int i = 0; i < 5; i++) {
            Vector3 randomPoint;
            try {
                randomPoint = transform.position + Random.insideUnitSphere * 10f;
            } catch (System.Exception e) {
                Debug.LogError(
                    $"{nameof(EnemyAI)} failed to generate a random point for summoning pawns: {e}"
                );
                yield break;
            }

            randomPoint.y = transform.position.y;

            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas)) {
                Debug.LogError(
                    $"{nameof(EnemyAI)} could not find a valid position to summon pawns"
                );
                yield break;
            }

            spawnManager.SpawnEnemy(EnemyType.Pawn, hit.position);

            yield return new WaitForSeconds(0.2f);
        }

        // enable movement
        agent.isStopped = false;

        yield return null;
    }

    IEnumerator Attack() {
        float startTime = Time.time;

        if (animator) animator.SetTrigger("Attack");

        while (Time.time - startTime < attackDuration) {
            // Debug.Log(Time.time - startTime);
            Collider[] hitColliders = Physics.OverlapSphere(
                transform.position + Vector3.forward,
                attackRange
            );
            if (hitColliders == null) yield break; // Exit the coroutine early if hitColliders is null

            foreach (var hitCollider in hitColliders) {
                if (hitCollider.gameObject == player) {
                    // playerHealth could be null, so we need to handle that case
                    playerHealth?.TakeDamage(attackDamage);
                    yield break; // Exit the coroutine early if the player is hit
                }
            }
            yield return null; // Wait for the next frame before continuing the loop
        }
    }

    GameObject FindClosestPawnCluster() {
        var pawns = GameObject.FindGameObjectsWithTag("Pawn").ToList();

        // Filter out already selected pawns
        pawns = pawns.Where(pawn => !PawnTargetManager.SelectedPawns.Contains(pawn)).ToList();
        if (pawns.Count == 0)
            return null; // Early exit if no available pawns

        int largestClusterSize = 0;
        Vector3 clusterCenter = Vector3.zero;
        List<GameObject> largestClusterPawns = new List<GameObject>();

        // Find the largest cluster and its center
        foreach (var pawn in pawns) {
            int clusterSize = 1; // Include the pawn itself
            Vector3 potentialClusterCenter = pawn.transform.position;
            List<GameObject> currentClusterPawns = new List<GameObject> { pawn };

            foreach (var otherPawn in pawns) {
                if (
                    pawn != otherPawn
                    && Vector3.Distance(pawn.transform.position, otherPawn.transform.position)
                        < clusterRadius
                ) {
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
            return targetPawn;
        }

        return null;
    }

    public bool isCharging = false;

    void HandleRookBehavior(float distanceToPlayer) {
        if (distanceToPlayer < attackRange) {
            TriggerAttack();
        } else if (
            !Physics.Raycast(
                transform.position,
                new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position, // locked on y axis
                out RaycastHit hit,
                distanceToPlayer,
                groundLayer
            ) && !isCharging && !chargingCooldown && 20f > Vector3.Distance(transform.position, player.transform.position) && Vector3.Distance(transform.position, player.transform.position) > 10f
          ) {
            // Charge at player
            StartCoroutine(ChargeTowardsPlayer());
        } else {
            agent.destination = player.transform.position;
        }
    }

    IEnumerator ChargeTowardsPlayer() {
        isCharging = true;
        agent.isStopped = true;

        Vector3 originalTargetPosition = new Vector3(
            player.transform.position.x,
            transform.position.y,
            player.transform.position.z
        );
        Vector3 targetPosition = originalTargetPosition; // Initial target position locked on y axis
        float totalChargeTime = 2f; // Total time to complete the charge
        float startTime = Time.time;

        transform.LookAt(targetPosition);

        animator.SetTrigger("Charge");
        yield return new WaitForSeconds(1.5f);

        SoundFXManager.Instance.Play(soundEffect, transform);

        while (isCharging) {
            targetPosition = new Vector3(
                player.transform.position.x,
                transform.position.y,
                player.transform.position.z
            );
            transform.LookAt(targetPosition);
            // set new position for frame
            transform.position += transform.forward * chargeSpeed * Time.deltaTime;

            if (Vector3.Distance(targetPosition, transform.position) <= 0.1f) {
                isCharging = false;
            }

            // Check if the charge time has elapsed
            if (Time.time - startTime >= totalChargeTime) {
                isCharging = false; // Exit the loop if the charge time has elapsed
            }

            yield return new WaitForEndOfFrame();
        }

        animator.SetTrigger("ChargeFinished");
        agent.isStopped = false;

        chargingCooldown = true;
        StartCoroutine(ChargeCooldown());

        // Set rook to nearest point on navmesh
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
            agent.destination = hit.position;
        }
    }

    public IEnumerator ChargeCooldown() {
        yield return new WaitForSeconds(chargingCooldownTime);

        chargingCooldown = false;
    }

    void GroundCheck() {
        if (!agent.isStopped)
            return;
        // if (Physics.Raycast(transform.position, Vector3.down, 1f)) {
        if (rigidbody != null && rigidbody.velocity.y == 0) {
            agent.isStopped = false;
            agent.updatePosition = true;
        }
    }
}

public static class PawnTargetManager {
    public static List<GameObject> SelectedPawns = new();

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
