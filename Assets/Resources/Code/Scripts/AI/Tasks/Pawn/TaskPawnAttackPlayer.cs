using System.Collections;
using BehaviorTree;
using UnityEngine;

public class TaskPawnAttackPlayer : AINode {
    readonly PlayerHealthSystem playerHealth;
    readonly Animator animator;
    readonly float cooldown;
    readonly int damage;

    bool waiting = false;

    public TaskPawnAttackPlayer(Animator animator, PlayerHealthSystem playerHealth, float cooldown, int damage) {
        this.playerHealth = playerHealth;
        this.animator = animator;
        this.cooldown = cooldown;
        this.damage = damage;
    }

    public override NodeState Evaluate() {
        if (waiting) return NodeState.FAILURE;

        animator.SetTrigger("Attack");
        playerHealth.TakeDamage(damage);
        waiting = true;
        GameManager.Instance.StartCoroutine(ResetCooldown());

        return NodeState.SUCCESS;
    }

    IEnumerator ResetCooldown() {
        yield return new WaitForSeconds(cooldown);
        waiting = false;
    }
}
