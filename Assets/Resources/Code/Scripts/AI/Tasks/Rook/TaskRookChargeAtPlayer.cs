using System.Collections;
using BehaviorTree;
using UnityEngine;
using UnityEngine.AI;

public class TaskRookChargeAtPlayer : AINode {
    readonly NavMeshAgent agent;
    readonly Animator animator;
    readonly Transform playerTransform;
    readonly float chargeSpeed;
    readonly float chargeCooldown;
    readonly Sound chargeSound;

    ChargeState chargeState = ChargeState.Idle;
    enum ChargeState { Charging, Cooldown, Idle }

    public TaskRookChargeAtPlayer(Animator animator, NavMeshAgent agent, Transform playerTransform, float chargeSpeed, float chargeCooldown, Sound chargeSound) {
        this.animator = animator;
        this.agent = agent;
        this.playerTransform = playerTransform;
        this.chargeSpeed = chargeSpeed;
        this.chargeCooldown = chargeCooldown;
        this.chargeSound = chargeSound;
    }

    public override NodeState Evaluate() {
        switch (chargeState) {
            case ChargeState.Charging:
                return NodeState.SUCCESS;

            case ChargeState.Cooldown:
                return NodeState.FAILURE;

            case ChargeState.Idle:
                agent.isStopped = false;
                animator.SetTrigger("Charge");
                chargeState = ChargeState.Charging;
                // SetData("isCharging", true);
                GameManager.Instance.StartCoroutine(Charge());
                return NodeState.RUNNING;
        }

        return NodeState.FAILURE;
    }

    IEnumerator Charge() {
        Vector3 target = new(playerTransform.position.x, agent.transform.position.y, playerTransform.position.z);
        agent.transform.LookAt(target);
        yield return new WaitForSeconds(1.5f);

        if (chargeSound != null)
            SoundFXManager.Instance.Play(chargeSound, agent.transform);

        while (chargeState == ChargeState.Charging) {
            agent.transform.position += chargeSpeed * Time.deltaTime * agent.transform.forward;

            if (Vector3.Distance(agent.transform.position, target) <= 0.1f) {
                chargeState = ChargeState.Cooldown;
                continue;
            }

            target = new(playerTransform.position.x, agent.transform.position.y, playerTransform.position.z);
            agent.transform.LookAt(target);
            yield return null;
        }

        animator.SetTrigger("ChargeFinished");
        // SetData("isCharging", false);
        // SetData("isOnCooldown", true);
        agent.isStopped = false;

        if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            agent.destination = hit.position;

        yield return new WaitForSeconds(chargeCooldown);

        chargeState = ChargeState.Idle;
        // SetData("isOnCooldown", false);
    }
}
