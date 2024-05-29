using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class BTPawn : AITree {
    public static float AttackRange { get; private set; } = 2f;
    public static int AttackDamage { get; private set; } = 1;
    public static float AttackDuration { get; private set; } = 2f;

    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agent;

    GameObject player;

    protected override AINode SetupTree() {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");

        // AINode root = new AISequence(new List<AINode> {
        //     new TaskChasePlayer(animator, agent, player.transform),
        //     new TaskAttackPlayer(animator, player.GetComponent<PlayerHealthSystem>())
        // });

        AINode root = new AISelector(new List<AINode> {
            new AISequence(new List<AINode> {
                new TaskCheckAttackRange(agent, player.transform),
                new TaskAttackPlayer(animator, player.GetComponent<PlayerHealthSystem>())
            }),
            new TaskChasePlayer(animator, agent, player.transform)
        });

        return root;
    }
}