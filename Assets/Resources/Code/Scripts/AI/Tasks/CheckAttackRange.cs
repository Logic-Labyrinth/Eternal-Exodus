using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckAttackRange : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;
    readonly float attackRange;

    public CheckAttackRange(NavMeshAgent agent, Transform playerTransform, float attackRange) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.attackRange = attackRange;
    }

    public override NodeState Evaluate() {
        if (Vector3.Distance(playerTransform.position, agent.transform.position) <= attackRange)
            return NodeState.SUCCESS;

        return NodeState.FAILURE;
    }
}
