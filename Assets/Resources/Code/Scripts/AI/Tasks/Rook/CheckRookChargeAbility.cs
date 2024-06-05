using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class CheckRookChargeAbility : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;
    readonly LayerMask groundLayer;

    public CheckRookChargeAbility(NavMeshAgent agent, Transform playerTransform) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    public override NodeState Evaluate() {
        bool linecast = Physics.Linecast(agent.transform.position, new Vector3(
            playerTransform.position.x, agent.transform.position.y, playerTransform.position.z
        ), groundLayer);

        if (!linecast) {
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
