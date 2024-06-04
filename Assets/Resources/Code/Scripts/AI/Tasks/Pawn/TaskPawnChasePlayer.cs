using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskPawnChasePlayer : AINode {
    readonly NavMeshAgent agent;
    readonly Transform playerTransform;
    readonly Animator animator;

    public TaskPawnChasePlayer(Animator animator, NavMeshAgent agent, Transform playerTransform) {
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.animator = animator;
    }

    public override NodeState Evaluate() {
        agent.SetDestination(playerTransform.position);

        if (!agent.hasPath)
            return NodeState.FAILURE;

        if (agent.remainingDistance <= agent.stoppingDistance) {
            animator.SetFloat("Speed", 0);
            return NodeState.SUCCESS;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        return NodeState.RUNNING;
    }
}
