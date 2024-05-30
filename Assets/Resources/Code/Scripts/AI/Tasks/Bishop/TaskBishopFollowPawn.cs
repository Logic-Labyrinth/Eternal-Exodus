using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskBishopFollowPawn : AINode {
    readonly NavMeshAgent agent;
    GameObject targetPawn;

    public TaskBishopFollowPawn(NavMeshAgent agent) {
        this.agent = agent;
    }

    public override NodeState Evaluate() {
        targetPawn = GetData("targetPawn") as GameObject;
        agent.SetDestination(targetPawn.transform.position);

        if (!agent.hasPath)
            return NodeState.FAILURE;

        if (agent.remainingDistance <= agent.stoppingDistance)
            return NodeState.SUCCESS;

        return NodeState.RUNNING;
    }
}
