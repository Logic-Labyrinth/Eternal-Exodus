using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class CheckBishopIfCloseToPlayer : AINode {
        readonly NavMeshAgent agent;
        readonly Transform    playerTransform;
        readonly float        fleeRange;

        public CheckBishopIfCloseToPlayer(NavMeshAgent agent, Transform playerTransform, float fleeRange) {
            this.agent           = agent;
            this.playerTransform = playerTransform;
            this.fleeRange       = fleeRange;
        }

        public override NodeState Evaluate() {
            if (Vector3.Distance(playerTransform.position, agent.transform.position) <= fleeRange)
                return NodeState.Success;

            return NodeState.Failure;
        }
    }
}