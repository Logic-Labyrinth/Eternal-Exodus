using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class TaskPawnChasePlayer : AINode {
        readonly NavMeshAgent agent;
        readonly Transform    playerTransform;
        readonly Animator     animator;
        readonly float        attackRange;

        public TaskPawnChasePlayer(Animator animator, NavMeshAgent agent, Transform playerTransform, float attackRange) {
            this.agent           = agent;
            this.playerTransform = playerTransform;
            this.animator        = animator;
            this.attackRange     = attackRange;
        }

        public override NodeState Evaluate() {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            if (!agent.hasPath)
                return NodeState.Failure;

            if (agent.remainingDistance <= attackRange) {
                agent.isStopped = true;
                animator.SetFloat("Speed", 0);
                return NodeState.Success;
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
            return NodeState.Running;
        }
    }
}