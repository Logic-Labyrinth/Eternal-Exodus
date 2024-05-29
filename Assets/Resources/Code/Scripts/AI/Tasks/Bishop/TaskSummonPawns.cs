using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskSummonPawns : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;
    readonly Animator animator;

    public TaskSummonPawns(Animator animator, NavMeshAgent agent, Transform playerTransform) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.animator = animator;
    }

    public override NodeState Evaluate() {
        agent.SetDestination(playerTransform.position);

        if(!agent.hasPath) {
            state = NodeState.FAILURE;
            return state;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) {
            // Reached player
            state = NodeState.SUCCESS;
            return state;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        state = NodeState.RUNNING;
        return state;
    }
}