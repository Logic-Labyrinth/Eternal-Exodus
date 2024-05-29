using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class BTBishop : AITree {
    [SerializeField] float retreatRange = 20f;
    [SerializeField] float projectileCooldown = 1f;
    [SerializeField] float summonCooldown = 1f;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;

    GameObject player;

    protected override AINode SetupTree() {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");

        AINode root = new AIBranch(
            new CheckIfCloseToPlayer(agent, player.transform, retreatRange),
            new TaskFleeFromPlayer(agent, player.transform, retreatRange),
            new AISelector(new List<AINode> {
                new TaskShootPlayer(agent, projectilePrefab, projectileCooldown)
            })
        );

        return root;
    }
}
