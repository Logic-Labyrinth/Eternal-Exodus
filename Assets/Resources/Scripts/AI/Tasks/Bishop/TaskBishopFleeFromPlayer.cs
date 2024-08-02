using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class TaskBishopFleeFromPlayer : AINode {
        readonly Transform    playerTransform;
        readonly NavMeshAgent agent;
        readonly float        fleeRange;

        public TaskBishopFleeFromPlayer(NavMeshAgent agent, Transform playerTransform, float fleeRange) {
            this.agent           = agent;
            this.playerTransform = playerTransform;
            this.fleeRange       = fleeRange;
        }

        public override NodeState Evaluate() {
            Vector3 fleeDestination = GetDirectionAwayFromPlayer();
            agent.SetDestination(fleeDestination);

            return NodeState.Running;
        }

        Vector3 GetDirectionAwayFromPlayer() {
            Vector3 direction = (agent.transform.position - playerTransform.position).normalized;

            float angle = Random.Range(-45f, 45f);
            direction = Quaternion.Euler(0, angle, 0) * direction;
            direction = playerTransform.position + direction * fleeRange;

            return direction;
        }
    }
}