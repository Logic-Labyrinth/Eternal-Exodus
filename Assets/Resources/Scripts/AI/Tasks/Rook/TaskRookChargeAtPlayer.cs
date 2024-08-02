using System.Collections;
using TEE.Audio;
using TEE.Enemies;
using UnityEngine;
using UnityEngine.AI;

namespace TEE.AI.Tasks {
    public class TaskRookChargeAtPlayer : AINode {
        readonly NavMeshAgent agent;
        readonly Animator     animator;
        readonly Transform    playerTransform;
        readonly float        chargeSpeed;
        readonly float        chargeCooldown;
        readonly Sound        chargeSound;
        readonly RookCharge   rookCharge;

        ChargeState         chargeState           = ChargeState.Idle;
        static readonly int AnimatorTriggerCharge = Animator.StringToHash("Charge");

        enum ChargeState {
            Charging,
            Cooldown,
            Idle
        }

        public TaskRookChargeAtPlayer(Animator animator, NavMeshAgent agent, Transform playerTransform, float chargeSpeed, float chargeCooldown, Sound chargeSound, RookCharge rookCharge) {
            this.animator        = animator;
            this.agent           = agent;
            this.playerTransform = playerTransform;
            this.chargeSpeed     = chargeSpeed;
            this.chargeCooldown  = chargeCooldown;
            this.chargeSound     = chargeSound;
            this.rookCharge      = rookCharge;
        }

        public override NodeState Evaluate() {
            switch (chargeState) {
                case ChargeState.Charging:
                    return NodeState.Success;

                case ChargeState.Cooldown:
                    return NodeState.Failure;

                case ChargeState.Idle:
                    agent.isStopped = true;
                    animator.SetTrigger(AnimatorTriggerCharge);
                    chargeState = ChargeState.Charging;
                    GameManager.Instance.StartCoroutine(Charge());
                    rookCharge.isCharging = true;
                    return NodeState.Running;
            }

            return NodeState.Failure;
        }

        IEnumerator Charge() {
            Vector3 target = new(playerTransform.position.x, agent.transform.position.y, playerTransform.position.z);
            agent.transform.LookAt(target);
            yield return new WaitForSeconds(1.5f);

            // if (chargeSound != null)
            //     SoundFXManager.Instance.Play(chargeSound, agent.transform);

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
            agent.isStopped = false;

            if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                agent.destination = hit.position;

            yield return new WaitForSeconds(chargeCooldown);

            chargeState = ChargeState.Idle;
        }
    }
}