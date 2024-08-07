using System.Collections.Generic;
using TEE.AI.Tasks;
using TEE.Audio;
using TEE.Enemy;
using TEE.Health;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.BT {
    public class BTRook : AITree {
        [SerializeField] int          updatesPerSecond = 1;
        [SerializeField] float        attackRange      = 2;
        [SerializeField] int          attackDamage     = 1;
        [SerializeField] float        attackDuration   = 2;
        [SerializeField] float        chargeCooldown   = 10;
        [SerializeField] float        chargeSpeed      = 20;
        [SerializeField] Animator     animator;
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Sound        chargeSound;
        [SerializeField] RookCharge   rCharge;

        GameObject player;

        protected override AINode SetupTree() {
            UpdatesPerSecond = updatesPerSecond;
            if (!agent) agent = GetComponent<NavMeshAgent>();
            player = GameObject.Find("Player");

            AINode root = new AISelector(new List<AINode> {
                new AISequence(new List<AINode> {
                    new CheckRookChargeAbility(agent, player.transform),
                    new TaskRookChargeAtPlayer(animator, agent, player.transform, chargeSpeed, chargeCooldown, chargeSound, rCharge)
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
}