using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    [SerializeField] private NavMeshAgent agent;
    private GameObject player;

    // Jump parameters
    private bool isJumping = false;
    public float jumpHeight = 2f; // Peak height of the jump
    public float jumpDuration = 1f; // Duration from start to finish
    public LayerMask groundLayer; // Ground layer to detect landing

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
    }

    void Update() {
        if (!isJumping) {
            agent.destination = player.transform.position; // Continuously try to move to the player

            if (ShouldJumpTowardsPlayer()) {
                JumpTowardsPlayer();
            }
        } else {
            CheckForLanding(); // Check if the AI has landed to complete the jump
        }
    }

    private void CheckForLanding() {
        // Raycast downwards to check for ground
        if (Physics.Raycast(transform.position, Vector3.down, 1.0f, groundLayer)) {
            CompleteJump();
        }
    }

    private void CompleteJump() {
        isJumping = false;
        agent.enabled = true; // Re-enable NavMeshAgent upon landing
        agent.SetDestination(player.transform.position);
    }

    private bool ShouldJumpTowardsPlayer() {
        // Calculate straight-line distance to player as jump cost
        float jumpCost = Vector3.Distance(transform.position, player.transform.position);
        float jumpCostThreshold = 10f; // Define a threshold that makes jumping worthwhile

        // Calculate navigation path length
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(player.transform.position, path);
        float navigationCost = CalculatePathLength(path);

        // Check if the player is within range and if jumping is more efficient than navigating
        return (jumpCost < jumpCostThreshold && jumpCost < navigationCost && jumpCost <= agent.remainingDistance) || (agent.pathStatus != NavMeshPathStatus.PathComplete && agent.remainingDistance <= 10f);
    }

    // Utility method to calculate the length of a navigation path
    private float CalculatePathLength(NavMeshPath path) {
        float length = 0f;
        if (path.corners.Length < 2) {
            return length;
        }

        for (int i = 0; i < path.corners.Length - 1; i++) {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    private bool CanJumpToTarget(Vector3 target) {
        Vector3 direction = (target - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target) / 2.0f; // Half distance for midpoint check
        float radius = 0.5f; // Adjust based on the size of your AI

        // Calculate the midpoint of the jump for the collision check
        Vector3 midpoint = transform.position + direction * distance;
        midpoint.y += jumpHeight / 2.0f; // Adjust for approximate peak height of the jump

        // SphereCast from AI's position to midpoint
        if (Physics.SphereCast(transform.position, radius, direction, out RaycastHit hit, distance, groundLayer)) {
            // Collision detected
            return false;
        }

        // Adjust and repeat the SphereCast for the second half of the jump if necessary
        // Note: This is a simplified approach. A full arc collision detection would require segment checks along the arc.

        return true; // No collision detected
    }

    private bool TryFindAlternativeJumpPoint(out Vector3 alternativePosition) {
        Vector3 originalTarget = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        float searchRadius = 5.0f; // Adjust based on how far you want the AI to search
        int numberOfPoints = 8; // Number of points around the AI to check

        for (int i = 0; i < numberOfPoints; i++) {
            float angle = (i * 360f) / numberOfPoints;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 potentialJumpPoint = transform.position + direction * searchRadius;

            if (CanJumpToTarget(potentialJumpPoint) && Physics.Raycast(potentialJumpPoint, Vector3.down, 2.0f)) { // Ensure ground is below
                alternativePosition = potentialJumpPoint;
                return true;
            }
        }

        alternativePosition = Vector3.zero;
        return false;
    }

    private void RepositionForJump() {
        if (TryFindAlternativeJumpPoint(out Vector3 alternativePosition)) {
            // Move the AI to the alternative position before jumping
            isJumping = true;
            agent.enabled = true;
            agent.SetDestination(alternativePosition);
            // You might set a flag or a timer to try jumping again after a delay or once the AI reaches the new position
        } else {
            // Handle case where no alternative position is found. Could involve waiting, signaling player unreachable, etc.
        }
    }

    private void JumpTowardsPlayer() {
        Vector3 targetPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        if (!CanJumpToTarget(targetPosition)) {
            RepositionForJump(); // Try to reposition before attempting the jump again
        } else {
            StartCoroutine(JumpToTarget(targetPosition)); // Proceed with the jump
        }
    }

    private IEnumerator JumpToTarget(Vector3 target) {
        isJumping = true;
        agent.enabled = false; // Disable the NavMeshAgent

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(target.x, transform.position.y, target.z);
        float heightDifference = target.y - startPosition.y;
        float characterHeight = GetComponent<Collider>().bounds.size.y; // Get the height of the character
        float clearanceHeight = 1.0f; // Additional clearance to avoid clipping
        float requiredPeakHeight = characterHeight + clearanceHeight + Mathf.Abs(heightDifference);
        float peakHeight = Mathf.Max(jumpHeight, requiredPeakHeight);

        float elapsedTime = 0;
        bool checkForGround = false; // Start checking for ground after the midpoint of the jump

        // Calculate the direction to the target
        Vector3 directionToTarget = (new Vector3(target.x, transform.position.y, target.z) - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Rotate towards the target before starting the jump
        float rotationSpeed = 2f; // Adjust the speed of rotation as needed
        float rotationTime = 0f;
        while (rotationTime < 1f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationTime);
            rotationTime += Time.deltaTime * rotationSpeed;
            yield return null;
        }
        transform.rotation = targetRotation; // Ensure the rotation is exactly aligned with the target


        while (elapsedTime < jumpDuration) {
            float ratio = elapsedTime / jumpDuration;
            Vector3 nextPosition = Vector3.Lerp(startPosition, targetPosition, ratio);

            float verticalRatio = (-4 * (ratio - 0.5f) * (ratio - 0.5f) + 1);
            nextPosition.y = startPosition.y + (peakHeight * verticalRatio) + (heightDifference * ratio);

            // Start checking for ground after passing the midpoint of the jump
            if (ratio > 0.5f) {
                checkForGround = true;
            }

            if (checkForGround && Physics.Raycast(nextPosition, Vector3.down, characterHeight / 2 + 0.1f, groundLayer)) {
                transform.position = nextPosition; // Update position to where ground contact was made
                CompleteJump();
                yield break; // Exit the coroutine early
            }

            transform.position = nextPosition;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Ensure precise landing if no ground contact was made earlier
        CompleteJump();
    }
}
