using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskWaypointPatrol : AINode {
    NavMeshAgent agentTransform;
    Transform[] waypoints;
    int currentWaypointIndex = 0;
    float waitTime = 1f;
    float waitTimer = 0f;
    bool waiting = false;

    public TaskWaypointPatrol(NavMeshAgent agentTransform, Transform[] waypoints) {
        this.agentTransform = agentTransform;
        this.waypoints = waypoints;
    }

    public override NodeState Evaluate() {
        if(waiting) {
            waitTimer += Time.fixedDeltaTime;
            if(waitTimer >= waitTime) {
                waiting = false;
                waitTimer = 0f;
            }
            return NodeState.RUNNING;
        } else {
            Transform waypoint = waypoints[currentWaypointIndex];
            if(Vector3.Distance(waypoint.position, agentTransform.transform.position) < 0.3f) {
                waiting = true;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            } else {
                agentTransform.SetDestination(waypoint.position);
            }
        }

        state = NodeState.RUNNING;
        return state;
    }
}