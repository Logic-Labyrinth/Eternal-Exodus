using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class BTPawn : AITree {
    [HideInInspector] public bool isFollowedByBishop = false;

    [SerializeField] float attackRange = 2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackDuration = 2f;
    [SerializeField] float targetCheckInterval = 3;
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
            new AIBranch(
                // Success = Inside the radius, Failure = Outside the radius
                new CheckPawnChasePointProximity(false, agent, player.transform),
                new AIBranch(
                    // Success = Inside the radius, Failure = Outside the radius
                    new CheckPawnChasePointProximity(true, agent, player.transform),
                    new TaskPawnChasePlayer(animator, agent, player.transform),
                    new AISequence(new List<AINode> {
                        new TaskPawnSelectChasePoint(true, agent, targetCheckInterval),
                        new TaskPawnChasePointAroundPlayer(animator, agent)
                    })
                ),
                new AISequence(new List<AINode> {
                    new TaskPawnSelectChasePoint(false, agent, targetCheckInterval),
                    new TaskPawnChasePointAroundPlayer(animator, agent)
                })
            )
        });

        return root;
    }
}
