using System.Collections;
using System.Collections.Generic;
using TEE.Player;
using UnityEngine;
using UnityEngine.AI;


namespace TEE.AI.Tasks {
    public class TaskPawnSelectChasePoint : AINode {
        readonly NavMeshAgent     agent;
        readonly List<GameObject> chasePoints;
        readonly float            checkInterval = 0.5f;

        bool waiting = false;

        public TaskPawnSelectChasePoint(bool inner, NavMeshAgent agent, float checkInterval) {
            this.agent         = agent;
            this.checkInterval = checkInterval;

            chasePoints = inner ? ChasePointCreation.Instance.InnerPoints : ChasePointCreation.Instance.OuterPoints;
        }

        public override NodeState Evaluate() {
            if (waiting) return NodeState.Running;

            chasePoints.Sort((a, b) => Vector3.Distance(agent.transform.position, a.transform.position).CompareTo(Vector3.Distance(agent.transform.position, b.transform.position)));
            int index = Random.Range(0, Mathf.CeilToInt(chasePoints.Count * 0.5f));

            Transform pointTransform = chasePoints[index].transform;
            Parent.SetData("chasePoint", pointTransform);
            waiting = true;
            GameManager.Instance.StartCoroutine(ResetCooldown());

            return NodeState.Success;
        }

        IEnumerator ResetCooldown() {
            yield return new WaitForSeconds(checkInterval);
            waiting = false;
        }
    }
}