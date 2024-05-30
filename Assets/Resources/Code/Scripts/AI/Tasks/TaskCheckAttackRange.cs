using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskCheckAttackRange : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;

    public TaskCheckAttackRange(NavMeshAgent agent, Transform playerTransform) {
        this.agent = agent;
        this.playerTransform = playerTransform;
    }

    public override NodeState Evaluate() {
        if (Vector3.Distance(playerTransform.position, agent.transform.position) <= BTPawn.AttackRange) {
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.FAILURE;
        return state;
    }
}
