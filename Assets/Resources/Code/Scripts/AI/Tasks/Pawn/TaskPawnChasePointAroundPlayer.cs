using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskPawnChasePointAroundPlayer : AINode {
    readonly NavMeshAgent agent;
    readonly Animator animator;
    Transform pointTransform;

    public TaskPawnChasePointAroundPlayer(Animator animator, NavMeshAgent agent) {
        this.animator = animator;
        this.agent = agent;
    }

    public override NodeState Evaluate() {
        pointTransform = GetData("chasePoint") as Transform;
        agent.SetDestination(pointTransform.position);

        if (!agent.hasPath) {
            animator.SetFloat("Speed", 0);
            return NodeState.FAILURE;
        }

        if (agent.remainingDistance <= agent.stoppingDistance) {
            animator.SetFloat("Speed", 0);
            return NodeState.SUCCESS;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
        return NodeState.RUNNING;
    }
}
