using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckIfCloseToPlayer : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;
    readonly float fleeRange;

    public CheckIfCloseToPlayer(NavMeshAgent agent, Transform playerTransform, float fleeRange) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.fleeRange = fleeRange;
    }

    public override NodeState Evaluate() {
        if(Vector3.Distance(playerTransform.position, agent.transform.position) <= fleeRange) {
            state = NodeState.SUCCESS;
            return state;
        } else {
            state = NodeState.FAILURE;
            return state;
        }
    }
}