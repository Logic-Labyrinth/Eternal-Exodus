using TEE.Player;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class CheckPawnChasePointProximity : AINode {
        readonly NavMeshAgent agent;
        readonly Transform    playerTransform;

        readonly bool  inner;
        readonly float innerPointRadius;
        readonly float innerPointRadiusOffset;
        readonly float outerPointRadius;
        readonly float outerPointRadiusOffset;

        public CheckPawnChasePointProximity(bool inner, NavMeshAgent agent, Transform playerTransform) {
            this.agent             = agent;
            this.playerTransform   = playerTransform;
            this.inner             = inner;
            innerPointRadius       = ChasePointCreation.Instance.innerPointRadius;
            innerPointRadiusOffset = ChasePointCreation.Instance.innerPointRadiusOffset;
            outerPointRadius       = ChasePointCreation.Instance.outerPointRadius;
            outerPointRadiusOffset = ChasePointCreation.Instance.outerPointRadiusOffset;
        }

        public override NodeState Evaluate() {
            float distance = GetDistanceToPlayer();
            if (inner && distance <= innerPointRadius + innerPointRadiusOffset)
                return NodeState.Success;

            if (!inner                                                &&
                distance <= outerPointRadius + outerPointRadiusOffset &&
                distance > innerPointRadius  + innerPointRadiusOffset)
                return NodeState.Success;

            return NodeState.Failure;
        }

        float GetDistanceToPlayer() {
            return Vector3.Distance(playerTransform.position, agent.transform.position);
        }
    }
}