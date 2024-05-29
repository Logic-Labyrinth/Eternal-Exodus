using BehaviorTree;
using UnityEngine;

public class TaskAttackPlayer : AINode {
    readonly PlayerHealthSystem playerHealth;
    readonly Animator animator;
    float timer = 0f;
    bool waiting = false;

    public TaskAttackPlayer(Animator animator, PlayerHealthSystem playerHealth) {
        this.playerHealth = playerHealth;
        this.animator = animator;
    }

    public override NodeState Evaluate() {
        // if (!(bool)parent.GetData("canHitPlayer")) return NodeState.FAILURE;
        if (waiting) {
            timer += Time.fixedDeltaTime;
            if (timer >= BTPawn.AttackDuration) {
                waiting = false;
                timer = 0f;
            }
            state = NodeState.RUNNING;
            return state;
        } else {
            if (animator) animator.SetTrigger("Attack");
            playerHealth.TakeDamage(BTPawn.AttackDamage);
            waiting = true;
            state = NodeState.SUCCESS;
            return state;
        }
    }
}