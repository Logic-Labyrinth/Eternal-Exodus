using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskShootPlayer : AINode {
    readonly NavMeshAgent agent;
    readonly GameObject projectilePrefab;
    readonly float projectileCooldown;
    float timer = 0;
    bool waiting = false;

    public TaskShootPlayer(NavMeshAgent agent, GameObject projectilePrefab, float projectileCooldown) {
        this.agent = agent;
        this.projectilePrefab = projectilePrefab;
        this.projectileCooldown = projectileCooldown;
    }

    public override NodeState Evaluate() {
        if(waiting) {
            timer += Time.fixedDeltaTime;
            if(timer >= projectileCooldown) {
                waiting = false;
                timer = 0f;
            }

            state = NodeState.RUNNING;
            return state;
        } else {
            waiting = true;
            Object.Instantiate(projectilePrefab, agent.transform.position + Vector3.up * 3.5f, Quaternion.identity);
            state = NodeState.SUCCESS;
            return state;
        }
    }
}