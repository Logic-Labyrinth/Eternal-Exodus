// using System.Collections;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using UnityEngine.AI;
// using Random = UnityEngine.Random;

// public enum EnemyType {
//     Pawn,
//     Bishop,
//     Knight,
//     Rook
// }

// public class EnemyAI : MonoBehaviour {
//     [SerializeField]
//     NavMeshAgent agent;
//     GameObject player;
//     public EnemyType enemyType = EnemyType.Pawn;

//     // Attack parameters
//     public float attackRange = 2f; // Radius of the attack sphere
//     public int attackDamage = 1; // Amount of damage to deal
//     public float attackDuration = 2f; // Duration of the attack in seconds

//     [SerializeField] LayerMask groundLayer;

//     [ShowIf("enemyType", EnemyType.Rook)] public float chargeSpeed = 20f;
//     [ShowIf("enemyType", EnemyType.Rook)] public float chargingCooldownTime = 10f;
//     bool chargingCooldown = false;

//     float checkInterval;

//     public Animator animator;
//     public Sound soundEffect;

//     new Rigidbody rigidbody;

//     void Start() {
//         agent = GetComponent<NavMeshAgent>();
//         player = GameObject.Find("Player");
//         checkInterval = Random.Range(0f, 0.1f);
//         rigidbody = GetComponent<Rigidbody>();
//     }

//     void OnEnable() {
//         checkInterval = Random.Range(0f, 0.1f);
//     }

//     // Time between checks in seconds
//     float lastCheckTime = 0f;

//     void FixedUpdate() {
//         // Reduce frequency of checks
//         if (Time.time >= lastCheckTime + checkInterval) {
//             DecisionMaker(enemyType);
//             lastCheckTime = Time.time;
//             checkInterval = Random.Range(0f, 0.2f);
//         }
//         GroundCheck();
//     }

//     void DecisionMaker(EnemyType type) {
//         float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

//         switch (type) {
//             case EnemyType.Rook:
//                 HandleRookBehavior(distanceToPlayer);
//                 break;
//         }
//     }

//     public bool isCharging = false;

//     void HandleRookBehavior(float distanceToPlayer) {
//         if (distanceToPlayer < attackRange) {
//             // TriggerAttack();
//         } else if (
//             !Physics.Raycast(
//                 transform.position,
//                 new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position, // locked on y axis
//                 out RaycastHit hit,
//                 distanceToPlayer,
//                 groundLayer
//             ) && !isCharging && !chargingCooldown && 20f > Vector3.Distance(transform.position, player.transform.position) && Vector3.Distance(transform.position, player.transform.position) > 10f
//           ) {
//             // StartCoroutine(ChargeTowardsPlayer());
//         } else {
//             agent.destination = player.transform.position;
//         }
//     }

//     // IEnumerator ChargeTowardsPlayer() {
//     //     isCharging = true;
//     //     agent.isStopped = true;

//     //     Vector3 originalTargetPosition = new(
//     //         player.transform.position.x,
//     //         transform.position.y,
//     //         player.transform.position.z
//     //     );

//     //     Vector3 targetPosition = originalTargetPosition; // Initial target position locked on y axis
//     //     float totalChargeTime = 2f; // Total time to complete the charge
//     //     float startTime = Time.time;

//     //     transform.LookAt(targetPosition);

//     //     animator.SetTrigger("Charge");
//     //     yield return new WaitForSeconds(1.5f);

//     //     SoundFXManager.Instance.Play(soundEffect, transform);

//     //     while (isCharging) {
//     //         targetPosition = new Vector3(
//     //             player.transform.position.x,
//     //             transform.position.y,
//     //             player.transform.position.z
//     //         );
//     //         transform.LookAt(targetPosition);
//     //         // set new position for frame
//     //         transform.position += chargeSpeed * Time.deltaTime * transform.forward;

//     //         if (Vector3.Distance(targetPosition, transform.position) <= 0.1f) {
//     //             isCharging = false;
//     //         }

//     //         // Check if the charge time has elapsed
//     //         if (Time.time - startTime >= totalChargeTime) {
//     //             isCharging = false; // Exit the loop if the charge time has elapsed
//     //         }

//     //         yield return new WaitForEndOfFrame();
//     //     }

//     //     animator.SetTrigger("ChargeFinished");
//     //     agent.isStopped = false;

//     //     chargingCooldown = true;
//     //     StartCoroutine(ChargeCooldown());

//     //     // Set rook to nearest point on navmesh
//     //     if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) {
//     //         agent.destination = hit.position;
//     //     }
//     // }

//     // public IEnumerator ChargeCooldown() {
//     //     yield return new WaitForSeconds(chargingCooldownTime);

//     //     chargingCooldown = false;
//     // }

//     void GroundCheck() {
//         if (!agent.isStopped) return;

//         if (rigidbody != null && rigidbody.velocity.y == 0) {
//             agent.isStopped = false;
//             agent.updatePosition = true;
//         }
//     }
// }
