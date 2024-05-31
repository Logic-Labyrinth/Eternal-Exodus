using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class TaskBishopFindClosestPawnCluster : AINode {
    readonly List<GameObject> pawns;
    readonly float clusterRadius = 10f;
    GameObject targetPawn;

    public TaskBishopFindClosestPawnCluster() {
        pawns = SpawnManager.Instance.pawnList;
    }

    public override NodeState Evaluate() {
        if (targetPawn && targetPawn.activeSelf)
            return NodeState.SUCCESS;

        if (SelectTargetPawn()) {
            parent.SetData("targetPawn", targetPawn);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }

    (List<GameObject>, Vector3) FindLargestCluster() {
        List<GameObject> cluster = new();
        int size = 0;
        Vector3 center = Vector3.zero;

        foreach (GameObject pawn in pawns) {
            int clusterSize = 1;
            Vector3 potentialCenter = pawn.transform.position;
            List<GameObject> currentCluster = new() { pawn };

            foreach (GameObject otherPawn in pawns) {
                if (pawn == otherPawn) continue;
                if (Vector3.Distance(pawn.transform.position, otherPawn.transform.position) < clusterRadius) {
                    clusterSize++;
                    currentCluster.Add(otherPawn);
                    potentialCenter += otherPawn.transform.position;
                }
            }

            if (clusterSize > size) {
                size = clusterSize;
                center = potentialCenter / clusterSize;
                cluster = new List<GameObject>(currentCluster);
            }
        }

        return (cluster, center);
    }

    bool SelectTargetPawn() {
        (List<GameObject> cluster, Vector3 center) = FindLargestCluster();
        GameObject target = null;
        float minDistance = float.MaxValue;

        foreach (GameObject pawn in cluster) {
            float distance = Vector3.Distance(pawn.transform.position, center);
            if (distance < minDistance && !pawn.GetComponent<BTPawn>().isFollowedByBishop) {
                minDistance = distance;
                target = pawn;
            }
        }

        if (target != null) {
            targetPawn = target;
            targetPawn.GetComponent<BTPawn>().isFollowedByBishop = true;
            return true;
        }

        return false;
    }
}
