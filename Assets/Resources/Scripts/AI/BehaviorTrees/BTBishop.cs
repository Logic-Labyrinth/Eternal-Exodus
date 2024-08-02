using System.Collections.Generic;
using TEE.AI.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.BT {
    public class BTBishop : AITree {
        [SerializeField] float        retreatRange       = 20f;
        [SerializeField] float        projectileCooldown = 1f;
        [SerializeField] float        summonCooldown     = 1f;
        [SerializeField] int          summonCount        = 5;
        [SerializeField] GameObject   projectilePrefab;
        [SerializeField] Animator     animator;
        [SerializeField] NavMeshAgent agent;

        GameObject player;

        protected override AINode SetupTree() {
            if (!agent) agent = GetComponent<NavMeshAgent>();
            player = GameObject.Find("Player");

            AINode root = new AIBranch(
                new CheckBishopIfCloseToPlayer(agent, player.transform, retreatRange),
                new TaskBishopFleeFromPlayer(agent, player.transform, retreatRange),
                new AISequence(new List<AINode> {
                    new AISequence(new List<AINode> {
                        new TaskBishopFindClosestPawnCluster(),
                        new TaskBishopFollowPawn(agent)
                    }),
                    new TaskBishopShootPlayer(agent, projectilePrefab, projectileCooldown),
                    new TaskBishopSummonPawns(animator, agent, summonCount, summonCooldown, retreatRange)
                })
            );

            return root;
        }
    }
}