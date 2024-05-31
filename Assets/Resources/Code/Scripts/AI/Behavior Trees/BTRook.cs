using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class BTRook : AITree {
    [SerializeField] float attackRange = 2;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackDuration = 2;
    [SerializeField] float chargeCooldown = 10;
    [SerializeField] float chargeSpeed = 20;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Sound chargeSound;

    GameObject player;

    protected override AINode SetupTree() {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");

        AINode root = new AISelector(new List<AINode> {
            new AISequence(new List<AINode> {
                new CheckRookChargeAbility(agent, player.transform),
                new TaskRookChargeAtPlayer(animator, agent, player.transform, chargeSpeed, chargeCooldown, chargeSound)
            }),
            new AISequence(new List<AINode> {
                new CheckAttackRange(agent, player.transform, attackRange),
                new TaskRookAttackPlayer(animator, player.GetComponent<PlayerHealthSystem>(), attackDuration, attackDamage)
            }),
            new TaskRookChasePlayer(animator, agent, player.transform)
        });

        return root;
    }
}
