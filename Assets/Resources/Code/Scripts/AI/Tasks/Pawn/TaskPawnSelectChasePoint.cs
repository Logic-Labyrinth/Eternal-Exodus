using System.Collections;
using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskPawnSelectChasePoint : AINode {
    readonly NavMeshAgent agent;
    readonly List<GameObject> chasePoints;
    readonly float checkInterval = 0.5f;

    bool waiting = false;

    public TaskPawnSelectChasePoint(NavMeshAgent agent, float checkInterval) {
        this.agent = agent;
        this.checkInterval = checkInterval;
        chasePoints = ChasePointCreation.Instance.Points;
    }

    public override NodeState Evaluate() {
        if (waiting) return NodeState.RUNNING;

        chasePoints.Sort((a, b) => Vector3.Distance(agent.transform.position, a.transform.position).CompareTo(Vector3.Distance(agent.transform.position, b.transform.position)));
        int index = Random.Range(0, Mathf.CeilToInt(chasePoints.Count * 0.5f));

        Transform pointTransform = chasePoints[index].transform;
        parent.SetData("chasePoint", pointTransform);
        waiting = true;
        GameManager.Instance.StartCoroutine(ResetCooldown());

        return NodeState.SUCCESS;
    }

    IEnumerator ResetCooldown() {
        yield return new WaitForSeconds(checkInterval);
        waiting = false;
    }
}