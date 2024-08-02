using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class TaskPawnChasePointAroundPlayer : AINode {
        readonly NavMeshAgent agent;
        readonly Animator     animator;
        Transform             pointTransform;

        public TaskPawnChasePointAroundPlayer(Animator animator, NavMeshAgent agent) {
            this.animator = animator;
            this.agent    = agent;
        }

        public override NodeState Evaluate() {
            pointTransform = GetData("chasePoint") as Transform;
            agent.SetDestination(pointTransform.position);

            if (!agent.hasPath) {
                animator.SetFloat("Speed", 0);
                return NodeState.Failure;
            }

            if (agent.remainingDistance <= agent.stoppingDistance) {
                animator.SetFloat("Speed", 0);
                return NodeState.Success;
            }

            animator.SetFloat("Speed", agent.velocity.magnitude);
            return NodeState.Running;
        }
    }
}