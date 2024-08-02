using System.Collections;
using TEE.Health;
using UnityEngine;

namespace TEE.AI.Tasks {
    public class TaskRookAttackPlayer : AINode {
        readonly PlayerHealthSystem playerHealth;
        readonly Animator           animator;
        readonly float              cooldown;
        readonly int                damage;

        bool                waiting               = false;
        static readonly int AnimatorTriggerAttack = Animator.StringToHash("Attack");

        public TaskRookAttackPlayer(Animator animator, PlayerHealthSystem playerHealth, float cooldown, int damage) {
            this.playerHealth = playerHealth;
            this.animator     = animator;
            this.cooldown     = cooldown;
            this.damage       = damage;
        }

        public override NodeState Evaluate() {
            if (waiting) return NodeState.Failure;

            animator.SetTrigger(AnimatorTriggerAttack);
            playerHealth.TakeDamage(damage);
            waiting = true;
            GameManager.Instance.StartCoroutine(ResetCooldown());

            return NodeState.Success;
        }

        IEnumerator ResetCooldown() {
            yield return new WaitForSeconds(cooldown);
            waiting = false;
        }
    }
}