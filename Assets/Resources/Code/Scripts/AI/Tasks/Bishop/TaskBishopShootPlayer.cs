using System.Collections;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskBishopShootPlayer : AINode {
    readonly NavMeshAgent agent;
    readonly GameObject projectilePrefab;
    readonly float projectileCooldown;
    bool waiting = false;

    public TaskBishopShootPlayer(NavMeshAgent agent, GameObject projectilePrefab, float projectileCooldown) {
        this.agent = agent;
        this.projectilePrefab = projectilePrefab;
        this.projectileCooldown = projectileCooldown;
    }

    public override NodeState Evaluate() {
        if (waiting) return NodeState.FAILURE;

        waiting = true;
        Object.Instantiate(projectilePrefab, agent.transform.position + Vector3.up * 3.5f, Quaternion.identity);
        GameManager.Instance.StartCoroutine(ResetCooldown());
        return NodeState.SUCCESS;
    }

    IEnumerator ResetCooldown() {
        yield return new WaitForSeconds(projectileCooldown);
        waiting = false;
    }
}
