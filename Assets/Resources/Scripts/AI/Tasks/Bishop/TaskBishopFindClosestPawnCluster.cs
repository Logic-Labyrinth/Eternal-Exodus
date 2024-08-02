using System.Collections.Generic;
using System.Linq;
using TEE.AI.BT;
using TEE.Enemies;
using UnityEngine;

namespace TEE.AI.Tasks {
    public class TaskBishopFindClosestPawnCluster : AINode {
        readonly List<GameObject> pawns         = SpawnManager.Instance.PawnList;
        readonly float            clusterRadius = 10f;
        GameObject                targetPawn;

        public override NodeState Evaluate() {
            if (targetPawn && targetPawn.activeSelf)
                return NodeState.Success;

            if (!SelectTargetPawn()) return NodeState.Failure;
            Parent.SetData("targetPawn", targetPawn);
            return NodeState.Success;
        }

        (List<GameObject>, Vector3) FindLargestCluster() {
            List<GameObject> cluster = new();
            int              size    = 0;
            Vector3          center  = Vector3.zero;

            foreach (var pawn in pawns) {
                int              clusterSize     = 1;
                Vector3          potentialCenter = pawn.transform.position;
                List<GameObject> currentCluster  = new() { pawn };

                foreach (var otherPawn in pawns.Where(otherPawn => pawn != otherPawn).Where(otherPawn => Vector3.Distance(pawn.transform.position, otherPawn.transform.position) < clusterRadius)) {
                    clusterSize++;
                    currentCluster.Add(otherPawn);
                    potentialCenter += otherPawn.transform.position;
                }

                if (clusterSize <= size) continue;
                size    = clusterSize;
                center  = potentialCenter / clusterSize;
                cluster = new List<GameObject>(currentCluster);
            }

            return (cluster, center);
        }

        bool SelectTargetPawn() {
            (List<GameObject> cluster, Vector3 center) = FindLargestCluster();
            GameObject target      = null;
            float      minDistance = float.MaxValue;

            foreach (var pawn in cluster) {
                float distance = Vector3.Distance(pawn.transform.position, center);
                if (!(distance < minDistance) || pawn.GetComponent<BTPawn>().isFollowedByBishop) continue;
                minDistance = distance;
                target      = pawn;
            }

            if (!target) return false;
            targetPawn                                           = target;
            targetPawn.GetComponent<BTPawn>().isFollowedByBishop = true;
            return true;
        }
    }
}