using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class TaskRookChasePlayer : AINode {
        readonly NavMeshAgent agent;
        readonly Transform    playerTransform;
        readonly Animator     animator;

        public TaskRookChasePlayer(Animator animator, NavMeshAgent agent, Transform playerTransform) {
            this.agent           = agent;
            this.playerTransform = playerTransform;
            this.animator        = animator;
        }

        public override NodeState Evaluate() {
            agent.SetDestination(playerTransform.position);

            if (!agent.hasPath)
                return NodeState.Failure;

            if (agent.remainingDistance <= agent.stoppingDistance) {
                // animator.SetFloat("Speed", 0);
                return NodeState.Success;
            }

            // animator.SetFloat("Speed", agent.velocity.magnitude);
            return NodeState.Running;
        }
    }
}