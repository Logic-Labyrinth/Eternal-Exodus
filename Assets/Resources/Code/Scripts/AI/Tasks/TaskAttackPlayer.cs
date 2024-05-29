using BehaviorTree;
using UnityEngine;

public class TaskAttackPlayer : AINode {
    readonly PlayerHealthSystem playerHealth;
    readonly Animator animator;
    readonly float attackDuration;
    readonly int attackDamage;
    float timer = 0f;
    bool waiting = false;

    public TaskAttackPlayer(Animator animator, PlayerHealthSystem playerHealth, float attackDuration, int attackDamage) {
        this.playerHealth = playerHealth;
        this.animator = animator;
        this.attackDuration = attackDuration;
        this.attackDamage = attackDamage;
    }

    public override NodeState Evaluate() {
        if (waiting) {
            timer += Time.fixedDeltaTime;
            if (timer >= attackDuration) {
                waiting = false;
                timer = 0f;
            }
            state = NodeState.RUNNING;
            return state;
        } else {
            if (animator) animator.SetTrigger("Attack");
            playerHealth.TakeDamage(attackDamage);
            waiting = true;
            state = NodeState.SUCCESS;
            return state;
        }
    }
}