using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckPawnChasePointProximity : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;

    public CheckPawnChasePointProximity(NavMeshAgent agent, Transform playerTransform) {
        this.agent = agent;
        this.playerTransform = playerTransform;
    }

    public override NodeState Evaluate() {
        if (GetDistanceToPlayer() <= ChasePointCreation.Instance.enemyRadius)
            return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }

    float GetDistanceToPlayer() {
        return Vector3.Distance(playerTransform.position, agent.transform.position);
    }
}
