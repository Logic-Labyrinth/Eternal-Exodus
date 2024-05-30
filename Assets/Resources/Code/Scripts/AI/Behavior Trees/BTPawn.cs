using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class BTPawn : AITree {
    [HideInInspector] public bool isFollowedByBishop = false;

    [SerializeField] float attackRange = 2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackDuration = 2f;
    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;

    GameObject player;

    protected override AINode SetupTree() {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");

        AINode root = new AISelector(new List<AINode> {
            new AISequence(new List<AINode> {
                new CheckAttackRange(agent, player.transform, attackRange),
                new TaskPawnAttackPlayer(animator, player.GetComponent<PlayerHealthSystem>(), attackDuration, attackDamage)
            }),
            new TaskPawnChasePlayer(animator, agent, player.transform)
        });

        return root;
    }
}
