using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskFleeFromPlayer : AINode {
    readonly Transform playerTransform;
    readonly NavMeshAgent agent;
    readonly float fleeRange;

    public TaskFleeFromPlayer(NavMeshAgent agent, Transform playerTransform, float fleeRange) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.fleeRange = fleeRange;
    }

    public override NodeState Evaluate() {
        Vector3 fleDestination = GetDirectionAwayFromPlayer();
        agent.SetDestination(fleDestination);

        state = NodeState.RUNNING;
        return state;
    }

    Vector3 GetDirectionAwayFromPlayer() {
        Vector3 direction = (playerTransform.position - agent.transform.position).normalized;

        float angle = Random.Range(-45f, 45f);
        direction = Quaternion.Euler(0, angle, 0) * direction;
        direction = playerTransform.position + direction * fleeRange;

        return direction;
    }
}